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
                Debug.LogError($"[BallShooter] InputAction '{_shootActionName}' not found.");
            }

            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var query = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<BallPrefabData>());

            if (query.IsEmpty) return; // _ballPrefab set check

            var holderEntity = query.GetSingletonEntity();
            var data = _entityManager.GetComponentData<BallPrefabData>(holderEntity);
            _ballPrefabEntity = data.Prefab;

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
            if ( _shootAction != null)
            {
                _shootAction.performed -= OnShootPerformed;
                _shootAction.Disable();
            }
        }

        private void OnShootPerformed(InputAction.CallbackContext ctx)
        {
            //if (_ballPrefabEntity == Entity.Null) return;

            var ballEntity = _entityManager.Instantiate(_ballPrefabEntity);

            var spawnPos = _spawnPoint != null ? _spawnPoint.position : transform.position;
            var spawnRot = _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

            var lt = LocalTransform.FromPositionRotationScale(spawnPos, spawnRot, 1f);

            _entityManager.SetComponentData(ballEntity, lt);

            if (_entityManager.HasComponent<PhysicsVelocity>(ballEntity))
            {
                float3 dir = math.normalize((float3)(_spawnPoint != null ? _spawnPoint.up : transform.up));
                var vel = _entityManager.GetComponentData<PhysicsVelocity>(ballEntity);
                vel.Linear = dir * _initialSpeed;
                _entityManager.SetComponentData(ballEntity, vel);
            }
        }
    }
}
