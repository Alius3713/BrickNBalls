using Brick_n_Balls.Core;
using Brick_n_Balls.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;

namespace Brick_n_Balls.ECS.Systems
{
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(BallOutOfBoundsSystem))]
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
                GameManager.Instance?.OnBallDestroyed();
                ecb.DestroyEntity(entity);
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }
    }

}
