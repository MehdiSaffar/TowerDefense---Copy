using UnityEngine;

public class Bullet : MonoBehaviour, IProjectile {
    [HideInInspector] public int hitDamage;
    [HideInInspector] public Enemy enemy;

    private float elapsedTime = 0f;

    [Header("Properties")]
    [SerializeField] private float lifetime;
    [SerializeField] private float speed;
    [SerializeField] private float radiusEpsilon;

    public int HitDamage
    {
        get
        {
            return hitDamage;
        }
        set
        {
            hitDamage = value;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed = value;
        }
    }
    public float Lifetime
    {
        get
        {
            return lifetime;
        }
        set
        {
            lifetime = value;
        }
    }

    void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;
    }
    private void Fsm_Changed(GameManager.States state)
    {
        switch (state)
        {
            case GameManager.States.MainEntry:
            case GameManager.States.MainMenu:
            case GameManager.States.Edit:
                Destroy(gameObject);
                enabled = false;
                break;
            case GameManager.States.Play:
                enabled = true;
                break;
            case GameManager.States.LevelLose:
            case GameManager.States.LevelWin:
            case GameManager.States.Pause:
                enabled = false;
                break;
        }
    }
    void OnDestroy()
    {
        GameManager.Fsm.Changed -= Fsm_Changed;
    }

    void FixedUpdate () {
        if(enemy == null)
        {
            Destroy(gameObject);
            return;
        }
        elapsedTime += Time.fixedDeltaTime;
        if(elapsedTime >= lifetime)
        {
            Destroy(gameObject);
            return;
        }
        if((enemy.transform.position - transform.position).magnitude <= radiusEpsilon)
        {
            enemy.TakeDamage(hitDamage);
            Destroy(gameObject);
            return;
        }
        transform.position += transform.forward * speed * Time.fixedDeltaTime;
	}
    void OnBecameInvisible() {
        Destroy(gameObject);
        return;
    }
}
