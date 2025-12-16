using Brick_n_Balls.Core;
using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Brick_n_Balls.Gameplay
{
    public class BrickSpawnRequest : MonoBehaviour
    {
        [Header("Brick Data")]
        [Tooltip("If empty, all direct children will be used as spawn points")]
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private BrickRegistry _brickRegistry;
        [SerializeField] private int _brickHp = 3;

        private EntityManager _entityManager;
        private Entity _brickPrefabEntity;
        private int _lastSpawnRoundId = -1;

        private void Awake()
        {
            AutoSetSpawnPointsIfValid();
            if (_brickRegistry == null) _brickRegistry = FindFirstObjectByType<BrickRegistry>();
        }

        public void SpawnForNewGame()
        {
            int roundId = GameManager.Instance.RoundId;
            if (_lastSpawnRoundId == roundId) return;

            if (!TryResolvePrefabEntity()) return;

            if (_brickRegistry != null) _brickRegistry.ClearAll(_entityManager);

            SpawnAll();

            _lastSpawnRoundId = roundId;
        }

        private void SpawnAll()
        {
            if (_brickPrefabEntity == Entity.Null)
            {
                Debug.LogError("[BrickSpawnRequest] Brick prefab entity is NULL");
                return;
            }

            foreach (var point in _spawnPoints)
            {
                if (point == null) continue;

                var brickEntity = _entityManager.Instantiate(_brickPrefabEntity);

                var transformData = LocalTransform.FromPositionRotationScale(point.position, point.rotation, 1f);

                _entityManager.SetComponentData(brickEntity, transformData);

                _brickRegistry?.Register(brickEntity, _brickHp);
            }
        }

        private void AutoSetSpawnPointsIfValid()
        {
            // --- Auto-set spawn points ---
            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                int childCount = transform.childCount;
                _spawnPoints = new Transform[childCount];

                for (int i = 0; i < childCount; i++)
                {
                    _spawnPoints[i] = transform.GetChild(i);
                }
            }
        }

        private bool TryResolvePrefabEntity()
        {
            if (_brickPrefabEntity != Entity.Null) return true;

            var world = World.DefaultGameObjectInjectionWorld;
            if (world == null || !world.IsCreated) return false;

            _entityManager = world.EntityManager;
            
            var query = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<BrickPrefabData>());
            if (query.IsEmpty) return false;

            var holder = query.GetSingletonEntity();
            var data = _entityManager.GetComponentData<BrickPrefabData>(holder);

            if (data.Prefab == Entity.Null) return false;

            _brickPrefabEntity = data.Prefab;
            return true;
        }
    }
}
