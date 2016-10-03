using UnityEngine;
using UnityEngine.UI;

public class PlayState : MonoBehaviour
{
    public AudioClip onWaveBegin;
    public AudioClip onSpawn;

    public UI.UIElement pauseButton;
    public UI.HealthBar healthBar;
    public UI.Money money;
    public UI.WaveIndicator waveIndicator;

    private float elapsedTime = 0f;
    public float waveIndicatorAnimationDuration = 1f;
    public void Enter()
    {
        // FIXME: Change pauseButton into UIElement ?
        pauseButton.isOpen = true;
        healthBar.isOpen = true;
        money.isOpen = true;

        pauseButton.GetComponent<Button>().onClick.AddListener(OnPauseClick);

        EventManager.BaseDie += OnBaseDie;

        if (GameManager.Fsm.LastState == GameManager.States.Edit) {
            waveIndicator.isOpen = true;
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
                SoundManager.RandomizeFx(onWaveBegin);
                waveIndicator.isOpen = false;
            }
        }
    }
    public void Exit()
    {
        pauseButton.isOpen = false;
        healthBar.isOpen = false;
        money.isOpen = false;
        waveIndicator.isOpen = false;

        pauseButton.GetComponent<Button>().onClick.RemoveListener(OnPauseClick);

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
