using Brick_n_Balls.Bridging;
using Brick_n_Balls.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.ECS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct BrickDestroyBridgeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BrickHitFlag>();
        }

        public void OnDestroy(ref SystemState state) { }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var entityManager = state.EntityManager;

            foreach (var (_, entity) in SystemAPI.Query<RefRO<BrickHitFlag>>().WithEntityAccess())
            {
                if (!entityManager.HasComponent<BrickView>(entity))
                {
                    Debug.LogWarning("[BrickDestroyBridgeSystem] Entity with BrickHitFlag has no BrickView – removing flag.");
                    ecb.RemoveComponent<BrickHitFlag>(entity);
                    continue;
                }

                var brickView = entityManager.GetComponentObject<BrickView>(entity);

                bool destroyed = brickView.ApplyHit();

                if (destroyed)
                {
                    Debug.Log("[BrickDestroyBridgeSystem] Brick died, destroying ECS entity.");
                    ecb.DestroyEntity(entity);
                }
                else
                {
                    Debug.Log("[BrickDestroyBridgeSystem] Brick survived hit, clearing BrickHitFlag.");
                    ecb.RemoveComponent<BrickHitFlag>(entity);
                }
            }

            ecb.Playback(entityManager);
            ecb.Dispose();
        }

    }
}
