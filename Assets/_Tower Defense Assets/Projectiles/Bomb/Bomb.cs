using UnityEngine;
using System.Collections;
using System;

public class Bomb : MonoBehaviour, IProjectile {

    [SerializeField] private int hitDamage;
    [SerializeField] private float lifetime;
    [SerializeField] private float speed;

    [Header("Properties")]
    public float blastRadius;
    public float targetDistance;

    [Header("FX")]
    public GameObject blastEffect;
    public AudioClip explosion;

    private float distanceFlown;
    private Vector3 oldScale;

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

    void Start () {
        distanceFlown = 0;
        oldScale = transform.localScale;
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
    void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
        distanceFlown += speed * Time.deltaTime;

        if(distanceFlown / targetDistance >= 0.8)
            transform.localScale = oldScale * Mathf.Lerp(1, 4, distanceFlown / targetDistance);
        if(distanceFlown >= targetDistance)
            BlowUp();
	}
    void BlowUp()
    {
        int remainingDamage = HitDamage;
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (var collider in colliders)
        {
            if (remainingDamage <= 0) break;

            Enemy enemy = collider.GetComponent<Enemy>();
            if(enemy)
                remainingDamage = enemy.TakeDamage(remainingDamage);
        }
        blastEffect = Instantiate(blastEffect, transform.position, blastEffect.transform.rotation) as GameObject;
        GameManager.SoundManager.RandomizeFx(explosion);
        Destroy(gameObject);
        enabled = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
