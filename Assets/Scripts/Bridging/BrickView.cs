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

        public void ApplyHit()
        {
            _currentHealth--;

            if( _currentHealth <= 0)
            {
                //notify ECS / destroy entity
                Destroy(gameObject);
            }
        }
    }
}
