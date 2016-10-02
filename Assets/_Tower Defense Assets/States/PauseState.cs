using UnityEngine;
using System.Collections;

public class PauseState : MonoBehaviour
{
    private UI.PauseMenu pauseMenu;
    public void Start()
    {
        pauseMenu = GUIManager.Instantiate(pauseMenu) as UI.PauseMenu;
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
