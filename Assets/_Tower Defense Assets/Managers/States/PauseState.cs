using UnityEngine;
using System.Collections;

public class PauseState : MonoBehaviour
{
    private PauseUIScript UIScript;
    public void Start()
    {
        UIScript = GUIManager.PauseMenu.GetComponent<PauseUIScript>();
        UIScript.KeepPlayingClick += OnKeepPlayingClick;
        UIScript.ReplayLevelClick += OnReplayLevelClick;
        UIScript.MainMenuClick += OnMainMenuClick;
    }
    public void Enter()
    {
        UIScript.gameObject.SetActive(true);
    }
    public void OnReplayLevelClick()
    {
        GameManager.LevelManager.ReloadLevel();
        GameManager.Fsm.ChangeState(GameManager.States.Edit);
    }
    public void OnKeepPlayingClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.Play);
    }
    public void OnMainMenuClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.MainMenu);
    }
    public void Exit()
    {
        UIScript.gameObject.SetActive(false);
    }
}
