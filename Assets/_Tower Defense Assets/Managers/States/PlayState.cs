using UnityEngine;
using UnityEngine.UI;

public class PlayState : MonoBehaviour
{
    public AudioClip onWaveBegin;
    public AudioClip onSpawn;

    private float elapsedTime = 0f;
    public float waveIndicatorAnimationDuration = 1f;
    public void Enter()
    {
        //GUIManager.TowerSelectionPanel.gameObject.SetActive(true);
        GUIManager.PauseButton.gameObject.SetActive(true);
        GUIManager.HealthBar.gameObject.SetActive(true);
        GUIManager.Money.gameObject.SetActive(true);
        GUIManager.WaveIndicator.gameObject.SetActive(true);

        GUIManager.PauseButton.onClick.AddListener(OnPauseClick);

        EventManager.BaseDie += OnBaseDie;

        if (GameManager.Fsm.LastState == GameManager.States.Edit) {
            GUIManager.WaveIndicator.GetComponent<Animator>().SetTrigger("Play");
            elapsedTime = 0f;
        }
    }
    public void Update()
    {
        if (GameManager.Fsm.LastState == GameManager.States.Edit)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= waveIndicatorAnimationDuration)
            {
                GameManager.LevelManager.waveSpawner.StartSpawning();
                GameManager.SoundManager.RandomizeFx(onWaveBegin);
            }
        }
    }
    public void Exit()
    {
        GUIManager.PauseButton.onClick.RemoveListener(OnPauseClick);

        //GUIManager.TowerSelectionPanel.gameObject.SetActive(false);
        GUIManager.HealthBar.gameObject.SetActive(false);
        GUIManager.Money.gameObject.SetActive(false);
        GUIManager.PauseButton.gameObject.SetActive(false);
        GUIManager.WaveIndicator.gameObject.SetActive(false);
    }
    public void OnBaseDie()
    {
        GameManager.Fsm.ChangeState(GameManager.States.LevelLose);
    }
    public void OnPauseClick()
    {
        GameManager.Fsm.ChangeState(GameManager.States.Pause);
    }
}
