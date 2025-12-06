using UnityEngine;

namespace Brick_n_Balls.Core
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private int _maxShots = 5;

        private int _shotsLeft;
        private int _activeBalls;
        private GameState _gameState;

        public GameState State => _gameState;
        public static GameManager Instance {  get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _shotsLeft = _maxShots;
            _activeBalls = 0;
            _gameState = GameState.Menu;
        }

        public void StartGame()
        {
            _shotsLeft = _maxShots;
            _activeBalls = 0;
            _gameState = GameState.Playing;

            // Later content...
        }

        public void OnBallSpawned()
        {
            _activeBalls++;
        }

        public void OnBallDestroyed()
        {
            _activeBalls--;

            if (_activeBalls <= 0 && _shotsLeft <= 0)
            {
                _gameState = GameState.GameOver; // notify UI to show popup window
            }
        }

        public bool CanShoot()
        {
            return _gameState == GameState.Playing && _shotsLeft > 0;
        }

        public void ConsumeShot()
        {
            if (_shotsLeft > 0)
            {
                _shotsLeft--;
            }
        }

        public int GetShotsLeft()
        {
            return _shotsLeft;
        }

        public void GoToMenu()
        {
            _gameState = GameState.Menu;
            _shotsLeft = _maxShots;
            _activeBalls = 0;
        }
    }
}
