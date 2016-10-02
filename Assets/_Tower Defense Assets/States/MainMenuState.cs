using UnityEngine;
using System.Collections;

public class MainMenuState : MonoBehaviour {
    private UI.MainMenu UIScript;
    public AudioClip backgroundMusic;

    public void OnPlayClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.LevelSelect);
    }
    public void Start()
    {
        UIScript.PlayClick += OnPlayClick;
    }
    public void Enter()
    {
        UIScript.gameObject.SetActive(true);
        if (GameManager.Fsm.LastState != GameManager.States.MainEntry)
            GameManager.LevelManager.UnbuildLevel();
        GameManager.SoundManager.SetMusic(backgroundMusic);
        GameManager.SoundManager.PlayMusic();
    }
    public void Exit()
    {
        UIScript.gameObject.SetActive(false);
    }
}