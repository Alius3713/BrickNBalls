using Brick_n_Balls.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Brick_n_Balls.Gameplay
{
    public class BallAimController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BallShootRequest _shooter;
        [SerializeField] private LineRenderer _aimLine;

        [Header("Aim Settings")]
        [Tooltip("Max left/right angle from base direction.")]
        [SerializeField] private float _maxAngleDeg = 75f;
        [Tooltip("How fast the aim rotates when moving mouse left/right.")]
        [SerializeField] private float _mouseSensitivity = 0.2f;
        [Tooltip("0 = no smoothing, higher = more smoothing.")]
        [SerializeField] private float _smoothing = 10f;

        [Header("Aim Line")]
        [SerializeField] private float _lineLength = 10f;
        [SerializeField] private bool _hideLineWhenCantShoot = true;

        private float _currentAngleDeg;
        private float _targetAngleDeg;

        public float CurrentAngleDeg => _currentAngleDeg;

        private void Reset()
        {
            _aimLine = GetComponentInChildren<LineRenderer>();
            _shooter = GetComponent<BallShootRequest>();
        }

        private void Awake()
        {
            if (_shooter == null) _shooter = GetComponent<BallShootRequest>();
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;

            bool canAim = GameManager.Instance.State == GameState.Playing;
            bool canShoot = GameManager.Instance.CanShoot();

            if (_aimLine != null)
            {
                _aimLine.enabled = !_hideLineWhenCantShoot ? canAim : (canAim && canShoot);
            }

            if (!canAim) return;

            UpdateAimFromMouseDelta();
            UpdateLine();
        }

        private void UpdateAimFromMouseDelta()
        {
            if (Mouse.current == null) return;

            float dx = Mouse.current.delta.ReadValue().x;
            _targetAngleDeg -= dx * _mouseSensitivity;
            _targetAngleDeg = Mathf.Clamp(_targetAngleDeg, -_maxAngleDeg, _maxAngleDeg);

            // --- smoothing ---
            if (_smoothing <= 0f)
            {
                _currentAngleDeg = _targetAngleDeg;
            }
            else
            {
                _currentAngleDeg = Mathf.Lerp(_currentAngleDeg, _targetAngleDeg, 1f - Mathf.Exp(-_smoothing * Time.deltaTime));
            }
        }

        public Vector3 GetShootDirWorld()
        {
            Transform sp = _shooter != null ? _shooter.SpawnPoint : transform;
            Vector3 baseDir = sp.up;
            Quaternion rot = Quaternion.AngleAxis(_currentAngleDeg, Vector3.forward);

            Vector3 dir = rot * baseDir;

            return dir.normalized;
        }

        private void UpdateLine()
        {
            if (_aimLine == null) return;

            Transform sp = _shooter != null ? _shooter.SpawnPoint : transform;
            Vector3 start = sp.position;
            Vector3 dir = GetShootDirWorld();
            Vector3 end = start + dir * _lineLength;

            _aimLine.positionCount = 2;
            _aimLine.SetPosition(0, start);
            _aimLine.SetPosition(1, end);
        }

        public void ResetAim()
        {
            _currentAngleDeg = 0f;
            _targetAngleDeg = 0f;
            UpdateLine();
        }
    }
}
