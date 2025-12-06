using Brick_n_Balls.Core;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Brick_n_Balls.UI
{
    public class UIRoot : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _gameHudPanel;
        [SerializeField] private GameObject _gameOverPanel;

        [Header("HUD Texts")]
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _shotsText;

        [Header("Game Over Texts")]
        [SerializeField] private TMP_Text _finalScoreText;

        private GameManager _gameManager;
        private ScoreManager _scoreManager;
        private bool _initialized;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _initialized = false;
        }

        private void Start()
        {
            ShowMainMenu();
        }

        private void Update()
        {
            if (!_initialized)
            {
                TryInit();
                if (!_initialized) return;
            }

            if (_gameManager.State == GameState.Playing)
            {
                UpdateHud();
            }

            if (_gameManager.State == GameState.GameOver && !_gameOverPanel.activeSelf)
            {
                ShowGameOver();
            }
        }

        private void TryInit()
        {
            if (GameManager.Instance == null || ScoreManager.Instance == null) return;

            _gameManager = GameManager.Instance;
            _scoreManager = ScoreManager.Instance;

            _initialized = true;

            switch (_gameManager.State)
            {
                case GameState.Menu:
                    ShowMainMenu();
                    break;
                case GameState.Playing:
                    ShowGameHud();
                    break;
                case GameState.GameOver:
                    ShowGameOver();
                    break;
            }
        }

        public void UpdateHud()
        {
            _scoreText.text = _scoreManager.Score.ToString();
            _shotsText.text = _gameManager.GetShotsLeft().ToString();
        }

        public void ShowMainMenu()
        {
            _mainMenuPanel.SetActive(true);
            _gameHudPanel.SetActive(false);
            _gameOverPanel.SetActive(false);
        }

        public void ShowGameHud()
        {
            _mainMenuPanel.SetActive(false);
            _gameHudPanel.SetActive(true);
            _gameOverPanel.SetActive(false);
        }

        public void ShowGameOver()
        {
            _mainMenuPanel.SetActive(false);
            _gameHudPanel.SetActive(false);
            _gameOverPanel.SetActive(true);

            if (_scoreManager != null)
            {
                _finalScoreText.text = _scoreManager.Score.ToString();
            }
        }

        public void OnStartGameClicked()
        {
            //Debug.Log("[UIRoot] Start Game clicked");
            if (!_initialized)
            {
                return;
            }

            _scoreManager.ResetScore();
            _gameManager.StartGame();
            ShowGameHud();
        }

        public void OnBackToMenuClicked()
        {
            if (!_initialized) return;

            _gameManager.GoToMenu();
            ShowMainMenu();
        }
    }
}