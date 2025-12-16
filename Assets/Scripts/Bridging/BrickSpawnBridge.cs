using Unity.Entities;
using Brick_n_Balls.ECS.Components;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    public class BrickSpawnBridge : MonoBehaviour
    {
        [Tooltip("BrickPrefab reference to spawn")]
        public GameObject BrickPrefab;

        class Baker : Baker<BrickSpawnBridge>
        {
            public override void Bake(BrickSpawnBridge authoring)
            {
                if (authoring.BrickPrefab == null) return;

                var entity = GetEntity(TransformUsageFlags.None);
                var prefabEntity = GetEntity(authoring.BrickPrefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new BrickPrefabData { Prefab = prefabEntity });
            }
        }
    }
}
