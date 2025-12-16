using Brick_n_Balls.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Brick_n_Balls.Gameplay
{
    public class GameplayEventPump : MonoBehaviour
    {
        [SerializeField] private BrickRegistry _brickRegistry;

        private EntityManager _em;
        private EntityQuery _hubQuery;
        private bool _hubQueryInitialized;

        private void Awake()
        {
            if (_brickRegistry == null) _brickRegistry = FindFirstObjectByType<BrickRegistry>();
        }

        private void Update()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;

            _em = world.EntityManager;

            if (!_hubQueryInitialized)
            {
                _hubQuery = _em.CreateEntityQuery(ComponentType.ReadOnly<BrickHitEventHubTag>(), ComponentType.ReadWrite<BrickHitEvent>());
                _hubQueryInitialized = true;
            }

            if (_hubQuery.IsEmpty) return;
            
            var hub = _hubQuery.GetSingletonEntity();
            var events = _em.GetBuffer<BrickHitEvent>(hub);
            if (events.Length == 0) return;
            
            // Limitation
            var unique = new NativeHashSet<Entity>(events.Length, Allocator.Temp);
            for (int i = 0; i < events.Length; i++)
            {
                unique.Add(events[i].Brick);
            }
            
            events.Clear();
            
            foreach (var brick in unique)
            {
                _brickRegistry?.ApplyHit(brick, _em);
            }
            
            unique.Dispose();
        }

        public void ResetRound()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;

            _em = world.EntityManager;

            _hubQuery = _em.CreateEntityQuery(ComponentType.ReadOnly<BrickHitEventHubTag>(), ComponentType.ReadWrite<BrickHitEvent>());

            if (_hubQuery.IsEmpty) return;

            var hub = _hubQuery.GetSingletonEntity();
            if (!_em.Exists(hub)) return;

            var events = _em.GetBuffer<BrickHitEvent>(hub);
            events.Clear();
        }

        private void OnDestroy()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return;
            if (!_hubQueryInitialized) return;

            _hubQuery.Dispose();
        }
    }
}
