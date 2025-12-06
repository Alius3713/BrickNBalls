using Brick_n_Balls.Bridging;
using Brick_n_Balls.ECS.Components;
using Unity.Collections;
using Unity.Entities;

namespace Brick_n_Balls.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BrickDestroyBridgeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrickDestroyFlag>();
        }

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entityManager = state.EntityManager;

            foreach (var (_, entity) in SystemAPI.Query<RefRO<BrickDestroyFlag>>().WithEntityAccess())
            {
                var bridge = entityManager.GetComponentObject<BrickPhysicsBridge>(entity);
                bridge.HandleBrickDestroyed();

                // score add in score manager

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(state.EntityManager);
        }

    }
}
