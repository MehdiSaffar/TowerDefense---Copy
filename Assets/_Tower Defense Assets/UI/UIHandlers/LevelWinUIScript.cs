using UnityEngine;
using System.Collections;

public class LevelWinUIScript : MonoBehaviour {
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
