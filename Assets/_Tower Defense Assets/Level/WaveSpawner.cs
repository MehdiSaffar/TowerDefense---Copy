using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [HideInInspector]
    public GameObject enemiesList;
    [HideInInspector]
    public WavechainData waveData;

    private float elapsedSinceWaveStart;
    private int health;

    // State booleans
    public bool askedToSpawn;
    public bool isPaused;

    // Health of base
    // TODO: Move health out of wave spawner ?
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = Mathf.Clamp(value, 0, 7);
            EventManager.TriggerHealthUpdate(health);
        }
    }

    // Wave related
    public int WaveIndex
    {
        get
        {
            return waveIndex;
        }
        set
        {
            waveIndex = value;
            EventManager.TriggerWaveIndexUpdate(waveIndex);
        }
    }
    private int waveIndex;
    private int waveletIndex;
    private int waveletRunningCount;

    public void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;

        enemiesList = new GameObject("Enemies List");
        enemiesList.transform.parent = GameManager.EntitiesList.transform;

        WaveIndex = 0;
        waveletIndex = 0;
        waveletRunningCount = 0;

        Health = 7;
        askedToSpawn = false;
        isPaused = false;
        elapsedSinceWaveStart = 0f;
    }
    private void Fsm_Changed(GameManager.States state)
    {
        isPaused = state != GameManager.States.Play;
    }
    void OnDestroy()
    {
        GameManager.Fsm.Changed -= Fsm_Changed;
        if (enemiesList != null)
        {
            Transform[] childrenObjects = enemiesList.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in childrenObjects) Destroy(child.gameObject);
        }
    }

    public void Update()
    {
        if (askedToSpawn && !isPaused)
        {
            elapsedSinceWaveStart += Time.deltaTime;

            if (waveletIndex < waveData.waves[WaveIndex].wavelets.Count)
            {
                WaveletData currentWavelet = waveData.waves[WaveIndex].wavelets[waveletIndex];
                if (elapsedSinceWaveStart >= currentWavelet.timeOffset)
                {
                    StartCoroutine(SpawnWavelet());
                    waveletIndex++;
                }
            }
            else
            {
                if (enemiesList.transform.childCount == 0)
                {
                    OnWaveFinished();
                }
            }
        }
    }

    private void OnWaveFinished()
    {
        WaveIndex++;
        waveletIndex = 0;
        elapsedSinceWaveStart = 0f;

        if (WaveIndex < waveData.waves.Count)
        {
            GameManager.Fsm.ChangeState(GameManager.States.Edit);
        }
        else
        {
            GameManager.Fsm.ChangeState(GameManager.States.LevelWin);
        }
        StopSpawning();
    }
    private void OnAllWavesFinished()
    {
        GameManager.Fsm.ChangeState(GameManager.States.LevelWin);
        StopSpawning();
    }
    public void StartSpawning()
    {
        askedToSpawn = true;
    }
    public void StopSpawning()
    {
        askedToSpawn = false;
    }
    public IEnumerator SpawnWavelet()
    {
        int thisWaveletIndex = waveletIndex;
        for (int i = 0; i < waveData.waves[WaveIndex].wavelets[thisWaveletIndex].enemiesCount; i++)
        {
            if (!askedToSpawn || isPaused)
                yield return new WaitWhile(IsPaused);
            GameObject enemy = Instantiate(
                EnemyManager.EnemyGO[waveData.waves[WaveIndex].wavelets[thisWaveletIndex].enemyType],
                transform.position,
                Quaternion.identity
                ) as GameObject;
            enemy.transform.position += Vector3.up * enemy.GetComponent<Renderer>().bounds.extents.y;
            enemy.transform.parent = enemiesList.transform;

            if (askedToSpawn)
                yield return new WaitForSeconds(1 / waveData.waves[WaveIndex].wavelets[thisWaveletIndex].spawnRate);
            else
                yield return new WaitWhile(IsPaused);
        }
    }
    private bool IsPaused()
    {
        return isPaused;
    }
}
