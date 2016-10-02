using UnityEngine;

public class LevelLoseState : MonoBehaviour
{
    private AudioSource audioSource;
    public UI.LevelLoseMenu UIScript;

    public AudioClip sound;

    void Start()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();
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
        UIScript.isOpen = true;
    }
    public void Exit()
    {
        UIScript.isOpen = false;
    }
}
