using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    [DisallowMultipleComponent]
    public class BrickPhysicsBridge : MonoBehaviour
    {
        class Baker : Baker<BrickPhysicsBridge>
        {
            public override void Bake(BrickPhysicsBridge authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<BrickTag>(entity);
            }
        }
    }
}
