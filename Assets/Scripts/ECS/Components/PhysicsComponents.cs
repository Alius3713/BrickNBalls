using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.ECS.Components
{
    public struct BallTag : IComponentData
    {

    }

    public struct BrickTag : IComponentData
    {

    }

    public struct WallTag : IComponentData
    {

    }

    public struct DestroyOnFall : IComponentData
    {
        public float YLimit;
    }

    public struct BrickHitFlag : IComponentData, IEnableableComponent
    {

    }

    public struct BallDestroyFlag : IComponentData
    {

    }

    public struct BallPrefabData : IComponentData
    {
        public Entity Prefab;
    }

    public struct BrickPrefabData : IComponentData
    {
        public Entity Prefab;
    }
}
