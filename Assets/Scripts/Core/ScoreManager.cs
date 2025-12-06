using UnityEngine;

namespace Brick_n_Balls.Core
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score;

        public int Score => _score;

        public void ResetScore()
        {
            _score = 0;
        }

        public void AddScore(int amount)
        {
            _score += amount;
        }
    }
}
