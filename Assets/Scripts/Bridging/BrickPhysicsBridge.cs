using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    [DisallowMultipleComponent]
    public class BrickPhysicsBridge : MonoBehaviour
    {
        [Range(1, 3)]
        public int maxHealth = 3;

        class Baker : Baker<BrickPhysicsBridge>
        {
            public override void Bake(BrickPhysicsBridge authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<BrickTag>(entity);
                AddComponent(entity, new BrickHealth { Value = Mathf.Clamp(authoring.maxHealth, 1, 3) });
            }
        }
    }
}
