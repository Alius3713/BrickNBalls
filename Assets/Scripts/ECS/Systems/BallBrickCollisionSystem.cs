using Brick_n_Balls.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Brick_n_Balls.ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))] // after simulation when collision events are ready 
    public partial struct BallBrickCollisionSystem : ISystem
    {
        private ComponentLookup<BallTag> _ballLookup;
        private ComponentLookup<BrickTag> _brickLookup;

        [BurstCompile]
        private struct BallBrickCollisionJob : ICollisionEventsJob
        {
            [ReadOnly] public ComponentLookup<BallTag> BallLookup;
            [ReadOnly] public ComponentLookup<BrickTag> BrickLookup;

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

                if (!BrickLookup.HasComponent(otherEntity)) return; // excluding non-brick collision
                
                Ecb.AddComponent<BrickHitFlag>(otherEntity); // tag as hit
                // notify +1 point in score manager
                Debug.Log($"[BallBrickCollisionSystem] Ball {ballEntity.Index} hit brick {otherEntity.Index} – added BrickHitFlag.");
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();

            _ballLookup = state.GetComponentLookup<BallTag>(isReadOnly: true);
            _brickLookup = state.GetComponentLookup<BrickTag>(isReadOnly: true);
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            _ballLookup.Update(ref state);
            _brickLookup.Update(ref state);

            var job = new BallBrickCollisionJob
            {
                BallLookup = _ballLookup,
                BrickLookup = _brickLookup,
                Ecb = ecb
            };

            state.Dependency = job.Schedule(simulation, state.Dependency);
            state.Dependency.Complete();

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
