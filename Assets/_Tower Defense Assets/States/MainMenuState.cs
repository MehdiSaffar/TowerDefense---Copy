using UnityEngine;
using System.Collections;

public class MainMenuState : MonoBehaviour {
#pragma warning disable 0649
    public UI.MainMenu UIScript;
#pragma warning restore 0649

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
        UIScript.isOpen = true;
        if (GameManager.Fsm.LastState != GameManager.States.MainEntry)
            GameManager.LevelManager.UnbuildLevel();
        SoundManager.SetMusic(backgroundMusic);
        SoundManager.PlayMusic();
    }
    public void Exit()
    {
        UIScript.isOpen = false;
    }
}