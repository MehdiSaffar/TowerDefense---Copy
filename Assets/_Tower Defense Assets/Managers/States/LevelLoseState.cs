using UnityEngine;
using System.Collections;

public class LevelLoseState : MonoBehaviour
{
    private AudioSource audioSource;
    private LevelLoseUIScript UIScript;

    public AudioClip sound;

    void Start()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
        UIScript = GUIManager.LevelLoseUI.GetComponent<LevelLoseUIScript>();
        UIScript.MainMenuClick += OnMainMenuClick;
        UIScript.ReplayLevelClick += OnReplayLevelClick;
    }
    public void OnReplayLevelClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.Edit);
    }
    public void OnMainMenuClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.MainMenu);
    }
    public void Enter()
    {
        audioSource.PlayOneShot(sound);
        UIScript.gameObject.SetActive(true);
    }
    public void Exit()
    {
        UIScript.gameObject.SetActive(false);
    }
}
