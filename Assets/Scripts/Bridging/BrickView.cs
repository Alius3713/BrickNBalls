using Brick_n_Balls.Core;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    public class BrickView : MonoBehaviour
    {
        [SerializeField] private int _maxHealth = 3;

        private int _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public bool ApplyHit()
        {
            _currentHealth--;

            ScoreManager.Instance?.AddScore(1);
            Debug.Log($"[BrickView] Hit! HP now = {_currentHealth}");

            if ( _currentHealth <= 0)
            {
                Debug.Log("[BrickView] Destroying brick GameObject");
                //notify ECS / destroy entity
                Destroy(gameObject);
                return true;
            }

            return false;
        }
    }
}
