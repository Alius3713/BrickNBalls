using Brick_n_Balls.Bridging;
using Brick_n_Balls.ECS.Components;
using Unity.Collections;
using Unity.Entities;

namespace Brick_n_Balls.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BallDestroyBridgeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BallDestroyFlag>();
        }

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entityManager = state.EntityManager;

            foreach (var (_, entity) in SystemAPI.Query<RefRO<BallDestroyFlag>>().WithEntityAccess())
            {
                var bridge = entityManager.GetComponentObject<BallPhysicsBridge>(entity);
                bridge.HandleBallDestroyed();

                // GameManager.Instance.OnBallDestroyed();

                ecb.DestroyEntity(entity);
            }

            ecb.Playback(entityManager);
        }
    }

}
