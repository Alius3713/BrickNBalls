using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    [DisallowMultipleComponent]
    public class BallPhysicsBridge : MonoBehaviour
    {
        [Tooltip("Y position below which balls will be destroyed.")]
        public float yDestroyLimit = -5f;

        [SerializeField] private BallView _ballView;

        class Baker : Baker<BallPhysicsBridge>
        {
            public override void Bake(BallPhysicsBridge authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<BallTag>(entity);
                AddComponent(entity, new DestroyOnFall { YLimit = authoring.yDestroyLimit });

                if (authoring._ballView != null)
                {
                    AddComponentObject(entity, authoring._ballView); // MonoBehaviour managed component attached to this entity
                } 
            }
        }
    }
}
