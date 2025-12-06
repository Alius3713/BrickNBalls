using Brick_n_Balls.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

namespace Brick_n_Balls.ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))] // after simulation when collision events are ready 
    public partial struct BallBrickCollisionSystem : ISystem
    {
        private ComponentLookup<BallTag> _ballLookup;
        private ComponentLookup<BrickTag> _brickLookup;
        private ComponentLookup<BrickHealth> _brickHealthLookup;

        [BurstCompile]
        private struct BallBrickCollisionJob : ICollisionEventsJob
        {
            [ReadOnly] public ComponentLookup<BallTag> BallLookup;
            public ComponentLookup<BrickTag> BrickLookup;
            public ComponentLookup<BrickHealth> BrickHealthLookup;

            public EntityCommandBuffer Ecb;

            public void Execute(CollisionEvent collisionEvent)
            {
                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                bool aIsBall = BallLookup.HasComponent(entityA);
                bool bIsBall = BallLookup.HasComponent(entityB);

                if (!(aIsBall ^ bIsBall)) return; // both balls or none excluded

                Entity ballEntity = aIsBall ? entityA : entityB;
                Entity otherEntity = aIsBall ? entityB : entityA;

                if (!BrickLookup.HasComponent(otherEntity) || !BrickHealthLookup.HasComponent(otherEntity)) return; // excluding non-brick collision

                BrickHealth health = BrickHealthLookup[otherEntity];
                health.Value -= 1;
                BrickHealthLookup[otherEntity] = health;

                if (health.Value <= 0)
                {
                    Ecb.DestroyEntity(otherEntity);
                    // notify +1 point in score manager
                }
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();

            _ballLookup = state.GetComponentLookup<BallTag>(isReadOnly: true);
            _brickLookup = state.GetComponentLookup<BrickTag>(isReadOnly: false);
            _brickHealthLookup = state.GetComponentLookup<BrickHealth>(isReadOnly: false);
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            _ballLookup.Update(ref state);
            _brickLookup.Update(ref state);
            _brickHealthLookup.Update(ref state);

            var job = new BallBrickCollisionJob
            {
                BallLookup = _ballLookup,
                BrickLookup = _brickLookup,
                BrickHealthLookup = _brickHealthLookup,
                Ecb = ecb
            };

            state.Dependency = job.Schedule(simulation, state.Dependency);
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
