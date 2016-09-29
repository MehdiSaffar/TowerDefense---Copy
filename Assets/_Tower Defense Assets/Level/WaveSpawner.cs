using UnityEngine;
using System.Collections;

public class WaveSpawner : MonoBehaviour
{
    [HideInInspector]
    public GameObject enemiesList;
    [HideInInspector]
    public WavechainData waveData;

    private float elapsedSinceWaveStart;
    public bool isSpawningWaves;
    private int waveIndex;
    private int waveletIndex;
    private int waveletRunningCount;
    private int health;

    // Setters for notifications
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

    public void Awake()
    {
        enemiesList = new GameObject("Enemies List");
        enemiesList.transform.parent = GameManager.EntitiesList.transform;
    }
    public void Start()
    {
        WaveIndex = 0;
        Health = 7;
        waveletIndex = 0;
        waveletRunningCount = 0;
        isSpawningWaves = false;
        elapsedSinceWaveStart = 0f;
        GameManager.Fsm.Changed += Fsm_Changed;
    }

    private void Fsm_Changed(GameManager.States state)
    {
        isSpawningWaves = state == GameManager.States.Play;
    }

    public void Update()
    {
        if (isSpawningWaves)
        {
            elapsedSinceWaveStart += Time.deltaTime;
            if (waveletIndex < waveData.waves[WaveIndex].wavelets.Count)
            {
                if (elapsedSinceWaveStart >= waveData.waves[WaveIndex].wavelets[waveletIndex].timeOffset)
                {
                    StartCoroutine(SpawnWavelet());
                    waveletIndex++;
                }
            }
            else
            {
                if (waveletRunningCount == 0 && enemiesList.transform.childCount == 0)
                {
                    isSpawningWaves = false;
                    WaveIndex++;
                    waveletIndex = 0;
                    elapsedSinceWaveStart = 0f;
                    if (WaveIndex == waveData.waves.Count)
                    {
                        GameManager.Fsm.ChangeState(GameManager.States.LevelWin);
                    }
                    else
                    {
                        GameManager.Fsm.ChangeState(GameManager.States.Edit);
                    }
                }
            }
        }
    }
    public void OnDestroy()
    {
        if (enemiesList != null)
        {
            Transform[] childrenObjects = enemiesList.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in childrenObjects) Destroy(child.gameObject);
        }
    }
    public void SpawnWave()
    {
        isSpawningWaves = waveData != null && WaveIndex < waveData.waves.Count;
    }
    public IEnumerator SpawnWavelet()
    {
        int thisWaveletIndex = waveletIndex;
        waveletRunningCount++;
        for (int i = 0; i < waveData.waves[WaveIndex].wavelets[thisWaveletIndex].enemiesCount; i++)
        {
            if (!isSpawningWaves)
                yield return new WaitUntil(IsSpawning);
            GameObject enemy = Instantiate(
                EnemyManager.EnemyGO[waveData.waves[WaveIndex].wavelets[thisWaveletIndex].enemyType],
                transform.position,
                Quaternion.identity
                ) as GameObject;
            enemy.transform.position += Vector3.up * enemy.GetComponent<Renderer>().bounds.extents.y;
            enemy.transform.parent = enemiesList.transform;
            if (isSpawningWaves)
                yield return new WaitForSeconds(1 / waveData.waves[WaveIndex].wavelets[thisWaveletIndex].spawnRate);
            else yield return new WaitUntil(IsSpawning);
        }
        waveletRunningCount--;
    }
    private bool IsSpawning()
    {
        return isSpawningWaves;
    }
}
