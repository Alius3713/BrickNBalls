using Unity.Entities;
using Brick_n_Balls.ECS.Components;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    [DisallowMultipleComponent]
    public class SpawnPointBridge : MonoBehaviour
    {
        [Tooltip("BallPrefab reference to spawn")]
        public GameObject BallPrefab;

        class Baker : Baker<SpawnPointBridge>
        {
            public override void Bake(SpawnPointBridge authoring)
            {
                if (authoring.BallPrefab == null) return;

                var entity = GetEntity(TransformUsageFlags.None);
                var prefabEntity = GetEntity(authoring.BallPrefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new BallPrefabData { Prefab = prefabEntity });
            }
        }
    }
}
