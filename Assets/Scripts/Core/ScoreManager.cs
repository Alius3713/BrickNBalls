using UnityEngine;

namespace Brick_n_Balls.Core
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score;

        public int Score => _score;

        public static ScoreManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void ResetScore()
        {
            _score = 0;
        }

        public void AddScore(int amount)
        {
            _score += amount;
            Debug.Log($"[ScoreManager] Score = {_score}");
        }
    }
}
