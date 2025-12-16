using Brick_n_Balls.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Brick_n_Balls.ECS.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))] // after simulation when collision events are ready 
    public partial struct BallBrickCollisionSystem : ISystem
    {
        private ComponentLookup<BallTag> _ballLookup;
        private ComponentLookup<BrickTag> _brickLookup;

        private Entity _eventHubEntity;

        [BurstCompile]
        private struct BallBrickCollisionJob : ICollisionEventsJob
        {
            [ReadOnly] public ComponentLookup<BallTag> BallLookup;
            [ReadOnly] public ComponentLookup<BrickTag> BrickLookup;

            public Entity EventHub;
            public EntityCommandBuffer Ecb;

            public void Execute(CollisionEvent collisionEvent)
            {
                if (EventHub == Entity.Null) return;

                Entity entityA = collisionEvent.EntityA;
                Entity entityB = collisionEvent.EntityB;

                bool aIsBall = BallLookup.HasComponent(entityA);
                bool bIsBall = BallLookup.HasComponent(entityB);

                if (!(aIsBall ^ bIsBall)) return; // both balls or none excluded

                Entity ballEntity = aIsBall ? entityA : entityB;
                Entity otherEntity = aIsBall ? entityB : entityA;

                if (!BrickLookup.HasComponent(otherEntity)) return; // excluding non-brick collision

                Ecb.AppendToBuffer(EventHub, new BrickHitEvent { Brick = otherEntity });
                //Ecb.SetComponentEnabled<BrickHitFlag>(otherEntity, true); // tag as hit
                // notify +1 point in score manager
            }
        }

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<PhysicsWorldSingleton>();

            _ballLookup = state.GetComponentLookup<BallTag>(isReadOnly: true);
            _brickLookup = state.GetComponentLookup<BrickTag>(isReadOnly: true);

            _eventHubEntity = Entity.Null;
        }

        public void OnDestroy(ref SystemState state)
        {
            if (state.EntityManager.Exists(_eventHubEntity)) state.EntityManager.DestroyEntity(_eventHubEntity);
        }

        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            if (_eventHubEntity == Entity.Null || !em.Exists(_eventHubEntity))
            {
                var query = em.CreateEntityQuery(ComponentType.ReadOnly<BrickHitEventHubTag>(), ComponentType.ReadWrite<BrickHitEvent>());
                if (!query.IsEmpty)
                {
                    _eventHubEntity = query.GetSingletonEntity();
                }
                else
                {
                    _eventHubEntity = em.CreateEntity();
                    em.AddComponent<BrickHitEventHubTag>(_eventHubEntity);
                    em.AddBuffer<BrickHitEvent>(_eventHubEntity);
                }
            }

            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

            _ballLookup.Update(ref state);
            _brickLookup.Update(ref state);

            var job = new BallBrickCollisionJob
            {
                BallLookup = _ballLookup,
                BrickLookup = _brickLookup,
                EventHub = _eventHubEntity,
                Ecb = ecb
            };

            state.Dependency = job.Schedule(simulation, state.Dependency);
            state.Dependency.Complete();

            ecb.Playback(em);
            ecb.Dispose();
        }

        public Entity GetEventHubEntity() => _eventHubEntity;
    }
}
