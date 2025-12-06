using Brick_n_Balls.Core;
using Brick_n_Balls.ECS.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Brick_n_Balls.Gameplay
{
    public class BallShootRequest : MonoBehaviour
    {
        [Header("Ball spawn")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _initialSpeed = 15f;

        [Header("Input (New Input System)")]
        [SerializeField] private string _shootActionName = "Attack";

        private InputAction _shootAction;

        private EntityManager _entityManager;
        private Entity _ballPrefabEntity;

        private void Awake()
        {
            _shootAction = InputSystem.actions?.FindAction(_shootActionName);
            if (_shootAction == null)
            {
                Debug.LogError($"[BallShootRequest] InputAction '{_shootActionName}' not found.");
            }

            if (World.DefaultGameObjectInjectionWorld != null)
            {
                _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            }
        }

        private void OnEnable()
        {
            if (_shootAction != null)
            {
                _shootAction.performed += OnShootPerformed;
                _shootAction.Enable();
            }
        }

        private void OnDisable()
        {
            if (_shootAction != null)
            {
                _shootAction.performed -= OnShootPerformed;
                _shootAction.Disable();
            }
        }

        /// <summary> Lazy resolve BallPrefabData </summary>
        private bool TryResolveBallPrefab()
        {
            if (_ballPrefabEntity != Entity.Null)
                return true;

            if (World.DefaultGameObjectInjectionWorld == null)
            {
                Debug.LogWarning("[BallShootRequest] World.DefaultGameObjectInjectionWorld is null – ECS world jeszcze nie istnieje.");
                return false;
            }

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var query = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<BallPrefabData>());
            if (query.IsEmpty)
            {
                Debug.LogWarning("[BallShootRequest] BallPrefabData not found yet. Czy SubScene z SpawnPointBridge + BallPrefab jest za³adowana?");
                return false;
            }

            var holderEntity = query.GetSingletonEntity();
            var data = _entityManager.GetComponentData<BallPrefabData>(holderEntity);
            _ballPrefabEntity = data.Prefab;

            Debug.Log($"[BallShootRequest] Resolved BallPrefab entity: {_ballPrefabEntity.Index}");
            return true;
        }

        private void OnShootPerformed(InputAction.CallbackContext ctx)
        {
            
            var gameManager = GameManager.Instance;
            if (gameManager != null && !gameManager.CanShoot())
            {
                Debug.Log("[BallShootRequest] Cannot shoot – GameManager.CanShoot() == false.");
                return;
            }

            if (!TryResolveBallPrefab())
            {
                Debug.LogWarning("[BallShootRequest] Cannot shoot – BallPrefabEntity not resolved yet.");
                return;
            }

            var ballEntity = _entityManager.Instantiate(_ballPrefabEntity);

            float3 spawnPos = _spawnPoint != null ? (float3)_spawnPoint.position : (float3)transform.position;
            Quaternion spawnRot = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

            var lt = LocalTransform.FromPositionRotationScale(spawnPos, spawnRot, 1f);
            _entityManager.SetComponentData(ballEntity, lt);

            if (_entityManager.HasComponent<PhysicsVelocity>(ballEntity))
            {
                float3 dir = math.normalize((float3)(_spawnPoint != null ? _spawnPoint.up : transform.up));

                var vel = _entityManager.GetComponentData<PhysicsVelocity>(ballEntity);
                vel.Linear = dir * _initialSpeed;
                _entityManager.SetComponentData(ballEntity, vel);
            }
            
            gameManager?.ConsumeShot();
        }
    }
}
