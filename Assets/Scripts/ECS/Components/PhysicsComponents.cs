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

    public struct BrickHealth : IComponentData
    {
        public int Value;
    }

    public struct DestroyOnFall : IComponentData
    {
        public float YLimit;
    }

    public struct BrickDestroyFlag : IComponentData
    {

    }

    public struct BallDestroyFlag : IComponentData
    {

    }
}
