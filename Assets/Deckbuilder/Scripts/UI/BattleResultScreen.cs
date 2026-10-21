using UnityEngine;
using TMPro;
namespace Deckbuilder.UI
{
    public class BattleResultScreen : MonoBehaviour
    {
        #region Variables
        
        private Canvas _canvas;
        
        [SerializeField] private TextMeshProUGUI resultTitle;
        [SerializeField] private TextMeshProUGUI resultMessage;
        
        #endregion
        
        
        #region Event Functions

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
        }
        
        #endregion
        
        
        #region Screen Setup

        public void ShowVictory()
        {
            _canvas.enabled = true;
            
            resultTitle.text = "VICTORY";
            resultMessage.text = "All enemies defeated!";
            gameObject.SetActive(true);
        }

        public void ShowDefeat()
        {
            _canvas.enabled = true;
            
            resultTitle.text = "DEFEAT";
            resultMessage.text = "You were defeated.";
            gameObject.SetActive(true);
        }

        #endregion
        
        
        #region UI Functions
        
        public void Restart()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            
#elif UNITY_WEBGL

#else
            Application.Quit();
#endif
        }
        
        #endregion
    }
}