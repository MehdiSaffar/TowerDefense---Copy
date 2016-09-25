using UnityEngine;
using UnityEngine.UI;

public class EditState : MonoBehaviour
{
    public Button playButton;

    public AudioClip editLoop;
    public AudioClip onSpawn;

    public void Start()
    {
        playButton = GUIManager.PlayButton.GetComponent<Button>();
    }
    public void Enter()
    {
        GameManager.SoundManager.SetMusic(editLoop);
        GameManager.SoundManager.PlayMusic();

        GUIManager.TowerSelectionPanel.gameObject.SetActive(true);
        GUIManager.Money.gameObject.SetActive(true);
        playButton.gameObject.SetActive(true);

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

        playButton.gameObject.SetActive(GameManager.LevelManager.towerList.transform.childCount > 0);
        playButton.onClick.AddListener(OnPlayClick);

        GameManager.LevelManager.TowerPlaced += OnTowerPlaced;
    }

    private void OnTowerPlaced()
    {
        playButton.gameObject.SetActive(true);
    }

    public void OnPlayClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.Play);
    }
    public void Exit()
    {
        GameManager.SoundManager.StopMusic();

        // FIXME: Find solution to changing state while placing tower

        playButton.gameObject.SetActive(false);
        playButton.onClick.RemoveListener(OnPlayClick);

        GameManager.LevelManager.TowerPlaced -= OnTowerPlaced;

        GUIManager.TowerSelectionPanel.gameObject.SetActive(false);
        GUIManager.Money.gameObject.SetActive(false);
    }
}
