using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    [DisallowMultipleComponent]
    public class BrickPhysicsBridge : MonoBehaviour
    {
        [SerializeField] private BrickView _brickView;

        class Baker : Baker<BrickPhysicsBridge>
        {
            public override void Bake(BrickPhysicsBridge authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<BrickTag>(entity);

                if (authoring._brickView != null)
                {
                    AddComponentObject(entity, authoring._brickView); // MonoBehaviour managed component attached to this entity
                }
            }
        }
    }
}
