using Brick_n_Balls.Core;
using UnityEngine;

namespace Brick_n_Balls.Bridging
{
    public class BallView : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance?.OnBallSpawned();
        }

        public void HandleOutOfBounds()
        {
            GameManager.Instance?.OnBallDestroyed();
            Destroy(gameObject);
        }
    }
}
