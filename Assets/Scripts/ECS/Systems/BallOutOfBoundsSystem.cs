using Brick_n_Balls.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Physics.Systems;

namespace Brick_n_Balls.ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    public partial struct BallOutOfBoundsSystem : ISystem
    {
        private ComponentLookup<BallDestroyFlag> _destroyFlagLookup;

        public void OnCreate(ref SystemState state) 
        {
            _destroyFlagLookup = state.GetComponentLookup<BallDestroyFlag>(true);
            state.RequireForUpdate<DestroyOnFall>();
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _destroyFlagLookup.Update(ref state);

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, destroyOnFall, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<DestroyOnFall>>().WithEntityAccess())
            {
                if (transform.ValueRO.Position.y < destroyOnFall.ValueRO.YLimit && !_destroyFlagLookup.HasComponent(entity))
                {
                    ecb.AddComponent<BallDestroyFlag>(entity); // tag as to destroy
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}
