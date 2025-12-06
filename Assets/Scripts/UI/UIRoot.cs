using UnityEngine;

namespace Brick_n_Balls.UI
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _gameHudPanel;
        [SerializeField] private GameObject _gameOverPanel;

        private void Awake()
        {
            ShowMainMenu();
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
        }
    }
}