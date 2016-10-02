using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
    [Header("Properties")]
    public float speed;
    public int value;
    public int health;
    [SerializeField] protected float waypointEpsilon;

    [Header("FX")]
    [SerializeField] protected ParticleSystem onKill;
    [SerializeField]
    protected UI.WorldHealthBar healthBar;
    [SerializeField] protected Vector3 healthBarWorldOffset;

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
        healthBar = GUIManager.Instantiate(healthBar) as UI.WorldHealthBar;
        healthBar.worldOffset = healthBarWorldOffset;
        healthBar.currentObject = gameObject;
        healthBar.SetMaxHealth(health);
        healthBar.isOpen = true;
    }
    private void Fsm_Changed(GameManager.States state)
    {
        enabled = state == GameManager.States.Play;
    }
    void OnDestroy()
    {
        GameManager.Fsm.Changed -= Fsm_Changed;
        Destroy(healthBar.gameObject);
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
        healthBar.OnHealthUpdate(health);
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
