using ModularForge.Trailback.Core;
using ModularForge.Trailback.Demo;
using ModularForge.Trailback.Demo.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Deckbuilder.UI
{
    public class HomeScreen : UIBase
    {
        #region Variables
        
        [SerializeField] private string battleSceneName;
        [SerializeField] private Canvas deckBuilderCanvas;
        
        #endregion
        
        
        #region UI Functions
        
        public void Play()
        {
            SceneManager.LoadScene(battleSceneName);
            TrailbackFacade.ResetHistory();
        }

        public void Exit()
        {
            TrailbackFacade.ResetHistory();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBGL

#else
            Application.Quit();
#endif
        }

        public void OpenAudioSetting()
        {
            DemoNavigationController.Instance.ShowScreen(DemoScreenType.About);
        }

        public void ConfigureDeck()
        {
            deckBuilderCanvas.enabled = true;
            DemoNavigationController.Instance.ShowScreen(DemoScreenType.Features);
        }
        
        #endregion
     
        
        #region Inherited Methods
        
        public override void Back()
        {
            base.Back();
            TrailbackFacade.Back();
        }
        
        #endregion
    }
}