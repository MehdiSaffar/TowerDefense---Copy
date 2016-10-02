using UnityEngine;
using UnityEngine.UI;

public class EditState : MonoBehaviour
{
#pragma warning disable 0649
    public UI.UIElement playButton;
    public UI.Money money;
#pragma warning restore 0649

    public AudioClip editLoop;
    public AudioClip onSpawn;

    public void Enter()
    {
        SoundManager.SetMusic(editLoop);
        SoundManager.PlayMusic();

        money.isOpen = true;

        switch (GameManager.Fsm.LastState)
        {
            case GameManager.States.MainEntry:
            case GameManager.States.MainMenu:
                Debug.Log("Loading and building Level1");
                GameManager.LevelManager.LoadLevel("Level1.dat");
                GameManager.LevelManager.BuildLevel();
                break;
            case GameManager.States.LevelWin:
            case GameManager.States.LevelLose:
                GameManager.LevelManager.ReloadLevel();
                break;
        }

        playButton.isOpen = GameManager.LevelManager.towerList.transform.childCount > 0;
        playButton.GetComponent<Button>().onClick.AddListener(OnPlayClick);

        GameManager.LevelManager.TowerPlaced += OnTowerPlaced;
    }
    private void OnTowerPlaced()
    {
        playButton.isOpen = true;
    }
    public void OnPlayClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.Play);
    }
    public void Exit()
    {
        SoundManager.StopMusic();

        playButton.GetComponent<Button>().onClick.RemoveListener(OnPlayClick);
        playButton.isOpen = false;

        GameManager.LevelManager.TowerPlaced -= OnTowerPlaced;

        money.isOpen = false;
    }
}
