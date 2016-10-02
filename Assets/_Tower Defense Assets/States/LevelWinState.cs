using UnityEngine;
using System.Collections;

public class LevelWinState : MonoBehaviour
{
#pragma warning disable 0649
    public UI.LevelWinMenu UIScript;
#pragma warning restore 0649

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
        SoundManager.RandomizeFx(sound);

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
