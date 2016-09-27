using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    [Header("Properties")]
    public float speed;
    public int value;
    public int health;
    [SerializeField] protected float waypointEpsilon;

    [Header("FX")]
    [HideInInspector] protected ParticleSystem onKill;

    [HideInInspector] public float distanceWalked;

    protected Vector3[] waypoints;
    protected int waypointIndex = 0;

    public void Awake()
    {
        waypointEpsilon *= waypointEpsilon;
        distanceWalked = 0f;
        waypoints = GameManager.LevelManager.GetUseableWaypoints();
    }
    public void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;
    }
    private void Fsm_Changed(GameManager.States state)
    {
        enabled = state == GameManager.States.Play;
    }
    void OnDestroy()
    {
        GameManager.Fsm.Changed -= Fsm_Changed;
    }

    public void Update()
    {
        if (waypointIndex >= waypoints.Length)
        {
            ReachedBase();
            return;
        }

        Vector3 dir = waypoints[waypointIndex] - transform.position;
        if (dir.sqrMagnitude <= waypointEpsilon) waypointIndex++;
        
        transform.LookAt(waypoints[waypointIndex]);
        distanceWalked += speed * Time.deltaTime;
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    public void ReachedBase()
    {
        GameManager.LevelManager.waveSpawner.Health--;
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    /// <summary>
    ///     Hits the enemy with `damage` and returns remaining damage
    /// </summary>
    /// <returns>Remaining damage</returns>
    public int TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            GameManager.Player.Money += value;
            if(onKill != null) Instantiate(onKill.gameObject, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            Destroy(gameObject);
            return -health;
        }
        return 0;
    }
}
