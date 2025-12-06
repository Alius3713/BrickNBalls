using UnityEngine;
using UnityEngine.SceneManagement;

namespace Brick_n_Balls.Core
{
    public class UISceneLoader : MonoBehaviour
    {
        [SerializeField] private string _uiSceneName = "UIscene";

        private void Start()
        {
            if (!SceneManager.GetSceneByName( _uiSceneName ).isLoaded)
            {
                SceneManager.LoadScene(_uiSceneName, LoadSceneMode.Additive);
            }
        }
    }
}
