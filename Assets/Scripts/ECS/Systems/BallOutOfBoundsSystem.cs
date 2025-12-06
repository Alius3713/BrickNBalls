using Brick_n_Balls.ECS.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;

namespace Brick_n_Balls.ECS.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BallOutOfBoundsSystem : ISystem
    {
        public void OnCreate(ref SystemState state) { }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transform, destroyOnFall, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<DestroyOnFall>>().WithEntityAccess())
            {
                if (transform.ValueRO.Position.y < destroyOnFall.ValueRO.YLimit)
                {
                    if (!state.EntityManager.HasComponent<BallDestroyFlag>(entity))
                    {
                        ecb.AddComponent<BallDestroyFlag>(entity); // tag as to destroy
                    }
                }
            }

            ecb.Playback(state.EntityManager);
        }
    }
}
