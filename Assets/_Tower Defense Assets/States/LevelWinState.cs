using UnityEngine;
using System.Collections;

public class LevelWinState : MonoBehaviour
{
    private UI.LevelWinMenu UIScript;
    public AudioClip sound;
    public void OnMainMenuClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.MainMenu);
    }
    public void OnReplayLevelClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.Edit);
    }
    public void Enter()
    {
        UIScript.isOpen = true;
        GameManager.SoundManager.RandomizeFx(sound);

        UIScript.MainMenuClick += OnMainMenuClick;
        UIScript.ReplayLevelClick += OnReplayLevelClick;

        GameManager.Player.data.lastUnlockedLevelId = Mathf.Max(GameManager.Player.data.lastUnlockedLevelId, GameManager.LevelManager.currentLevelData.id + 1);
    }
    public void Exit()
    {
        UIScript.MainMenuClick -= OnMainMenuClick;
        UIScript.ReplayLevelClick -= OnReplayLevelClick;
        UIScript.isOpen = false;
    }
}
