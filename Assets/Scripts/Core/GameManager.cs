using Brick_n_Balls.Gameplay;
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
        public int ShotsLeft => _shotsLeft;
        public int ActiveBalls => _activeBalls;

        public static GameManager Instance {  get; private set; }
        public int RoundId { get; private set; }

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
            RoundId++;
            _gameState = GameState.Playing;
            FindFirstObjectByType<BrickSpawnRequest>()?.SpawnForNewGame();
            _shotsLeft = _maxShots;
            _activeBalls = 0;
        }

        public void OnBallSpawned()
        {
            if (_gameState != GameState.Playing) return;

                _activeBalls++;
            Debug.Log($"[GameManager] Ball spawned. ActiveBalls = {_activeBalls}, ShotsLeft = {_shotsLeft}");
        }

        public void OnBallDestroyed()
        {
            if (_activeBalls > 0) _activeBalls--;
            Debug.Log($"[GameManager] Ball destroyed. ActiveBalls = {_activeBalls}, ShotsLeft = {_shotsLeft}");

            if (_gameState != GameState.Playing) return;

            if (_activeBalls == 0 && _shotsLeft == 0)
            {
                GameOver(); // notify UI to show popup window
            }
        }

        private void GameOver()
        {
            _gameState = GameState.GameOver;
            Debug.Log("[GameManager] GAME OVER");
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
            FindFirstObjectByType<GameplayEventPump>()?.ResetRound();
            _shotsLeft = _maxShots;
            _activeBalls = 0;
        }
    }
}
