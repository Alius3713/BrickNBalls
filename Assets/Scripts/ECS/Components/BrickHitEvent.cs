using Unity.Entities;

namespace Brick_n_Balls.ECS.Components
{
    public struct BrickHitEvent : IBufferElementData
    {
        public Entity Brick;
    }

    public struct BrickHitEventHubTag : IComponentData { }
}
