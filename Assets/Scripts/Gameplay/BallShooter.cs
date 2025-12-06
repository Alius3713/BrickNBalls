using Brick_n_Balls.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Brick_n_Balls.Gameplay
{
    public class BallShooter : MonoBehaviour
    {
        [Header("Ball spawn")]
        [SerializeField] private GameObject _ballPrefab;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private float _initialSpeed = 15f;

        private InputAction _shootAction;

        private void Awake()
        {
            _shootAction = InputSystem.actions.FindAction("Attack");
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
            Debug.Log("SHOOT PERFORMED!");
            var gameManager = GameManager.Instance;
            if (gameManager == null || !gameManager.CanShoot()) return;

            gameManager.ConsumeShot();

            GameObject ballInstance = Object.Instantiate(_ballPrefab, _spawnPoint.position, Quaternion.identity);

            if (ballInstance.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 direction = transform.up; // change to mouse look input dir

                rb.linearVelocity = direction.normalized * _initialSpeed;
            }
        }
    }
}
