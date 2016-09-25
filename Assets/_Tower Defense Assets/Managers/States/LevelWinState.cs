using UnityEngine;
using System.Collections;

public class LevelWinState : MonoBehaviour
{
    private LevelWinUIScript UIScript;
    public AudioClip sound;
    void Start()
    {
        UIScript = GUIManager.LevelWinUI;
    }
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
        UIScript.gameObject.SetActive(true);
        GameManager.SoundManager.RandomizeFx(sound);

        UIScript.MainMenuClick += OnMainMenuClick;
        UIScript.ReplayLevelClick += OnReplayLevelClick;

        GameManager.Player.data.lastUnlockedLevelId = Mathf.Max(GameManager.Player.data.lastUnlockedLevelId, GameManager.LevelManager.currentLevelData.id + 1);
    }
    public void Exit()
    {
        UIScript.MainMenuClick -= OnMainMenuClick;
        UIScript.ReplayLevelClick -= OnReplayLevelClick;
        UIScript.gameObject.SetActive(false);
    }
}
