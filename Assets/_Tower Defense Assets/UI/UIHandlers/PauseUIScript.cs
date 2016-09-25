using UnityEngine;
using System.Collections;

public class PauseUIScript : MonoBehaviour {

    public delegate void OnReplayLevelClick();
    public delegate void OnMainMenuClick();
    public delegate void OnKeepPlayingClick();

    public event OnReplayLevelClick ReplayLevelClick;
    public event OnMainMenuClick MainMenuClick;
    public event OnKeepPlayingClick KeepPlayingClick;

    public void TriggerReplayLevelClick()
    {
        ReplayLevelClick();
    }
    public void TriggerMainMenuClick()
    {
        MainMenuClick();
    }
    public void TriggerKeepPlayingClick()
    {
        KeepPlayingClick();
    }
}
