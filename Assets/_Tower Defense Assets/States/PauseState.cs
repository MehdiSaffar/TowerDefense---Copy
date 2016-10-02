using UnityEngine;
using System.Collections;

public class PauseState : MonoBehaviour
{
#pragma warning disable 0649
    public UI.PauseMenu pauseMenu;
#pragma warning restore 0649

    public void Start()
    {
        pauseMenu.KeepPlayingClick += OnKeepPlayingClick;
        pauseMenu.ReplayLevelClick += OnReplayLevelClick;
        pauseMenu.MainMenuClick    += OnMainMenuClick;
    }
    public void Enter()
    {
        pauseMenu.isOpen = true;
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
        pauseMenu.isOpen = false;
    }
}
