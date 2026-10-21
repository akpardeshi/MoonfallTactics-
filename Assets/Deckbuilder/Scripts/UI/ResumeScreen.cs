using UnityEngine;

namespace Deckbuilder.Scripts.UI
{
    public class ResumeScreen : MonoBehaviour
    {
        #region Variables
        
        private Canvas _canvas;
        
        #endregion
        
        
        #region Event Functions

        private void Awake()
        {
            TryGetComponent(out _canvas);
        }
        
        #endregion
        
        
        #region UI Functions

        public void Resume()
        {
            Time.timeScale = 1;
            _canvas.enabled = false;
        }

        public void Home()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public void Restart()
        {
            Time.timeScale = 1;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
        
        #endregion
    }
}