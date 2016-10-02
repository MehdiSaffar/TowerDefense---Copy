using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelLoseMenu : UIElement
    {
        public delegate void OnReplayLevelClick();
        public delegate void OnMainMenuClick();

        public event OnReplayLevelClick ReplayLevelClick;
        public event OnMainMenuClick MainMenuClick;

        public void TriggerReplayLevelClick()
        {
            ReplayLevelClick();
        }
        public void TriggerMainMenuClick()
        {
            MainMenuClick();
        }

        public Image background;

        public void Start()
        {
            background.CrossFadeAlpha(0, 0, false);
            background.CrossFadeAlpha(255, 3, false);
        }
    }
}