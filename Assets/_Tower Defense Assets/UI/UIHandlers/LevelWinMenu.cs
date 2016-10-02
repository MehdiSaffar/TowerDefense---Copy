using UnityEngine;
using System.Collections;
namespace UI
{
    public class LevelWinMenu : UIElement
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
    }
}