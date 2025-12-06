using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    [DisallowMultipleComponent]
    public class WallPhysicsBridge : MonoBehaviour
    {
        class Baker : Baker<WallPhysicsBridge>
        {
            public override void Bake(WallPhysicsBridge authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<WallTag>(entity);
            }
        }
    }
}
