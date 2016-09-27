using UnityEngine;
using System.Collections.Generic;

/*public interface ITower
{
    TowerData Data { get; set; }
    int Cost { get; set; }
    float Range { get; set; }
    GameObject RangeGizmo { get; set; }
    float TurnSpeed { get; set; }
    float HitRate { get; set; }
    float HitDamage { get; set; }
    float AngleEpsilon { get; set; }
    float SellPrice { get; }

    bool CanUpgrade();
}*/

public class Tower : MonoBehaviour {
    public enum AimBehaviour
    {
        Closest = 0,
        Farthest,
        Healthiest,
    };
    public TowerData data;
    public int cost;

    [Header("Parts")]
    public GameObject head;
    public Transform[] muzzleEnd;
    public GameObject projectile;

    [Header("Visual")]
    public Color selectionHighlightColor;
    public GameObject rangeGizmo;
    [SerializeField] protected ParticleSystem muzzleFire;

    [Header("Sound FX")]
    [SerializeField] protected AudioClip onFire;

    [Header("Properties")]
    public AimBehaviour aimBehaviour;
    /// <summary>
    /// Number of projectiles per minute
    /// </summary>
    public int hitRate; 
    public int hitDamage;
    public float range;
    public float turnSpeed; 
    [SerializeField] protected float angleEpsilon;

    protected Enemy currentTarget = null;
    protected List<ParticleSystem> _muzzleFire = new List<ParticleSystem>();
    protected float elapsedSinceFire = 0f;

    void Awake() {
        if (muzzleFire != null) {
            for (int i = 0; i < muzzleEnd.Length; i++)
            {
                ParticleSystem tempMuzzleFire = (Instantiate(muzzleFire.gameObject, muzzleEnd[i].transform.position, muzzleEnd[i].transform.rotation) as GameObject).GetComponent<ParticleSystem>();
                tempMuzzleFire.transform.parent = muzzleEnd[i];
                _muzzleFire.Add(tempMuzzleFire);
            }
        }
        if (rangeGizmo)
        {
            rangeGizmo = Instantiate(rangeGizmo, transform.position, Quaternion.identity) as GameObject;
            rangeGizmo.transform.parent = transform;
            Vector3 newScale = rangeGizmo.transform.localScale * range;
            newScale.y = 3;
            rangeGizmo.transform.localScale = newScale;
            rangeGizmo.SetActive(false);
        }
        foreach (Highlightable highlightable in GetComponentsInChildren<Highlightable>(true))
            highlightable.highlightColor = selectionHighlightColor;
    }
    void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;
    }
    void Fsm_Changed(GameManager.States state)
    {
        enabled = state == GameManager.States.Play;
    }
    void OnDestroy()
    {
        GameManager.Fsm.Changed -= Fsm_Changed;
    }

    public int GetSellPrice()
    {
        return Mathf.RoundToInt(cost * GameManager.TowerManager.sellPercentage);
    }

    protected Enemy GetEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject target = null;
        switch (aimBehaviour)
        {
            case AimBehaviour.Closest:
                {
                    float shortestSqrDistance = range * range;
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        float currentSqrDistance = (enemies[i].transform.position - head.transform.position).sqrMagnitude;
                        if (currentSqrDistance < shortestSqrDistance)
                        {
                            shortestSqrDistance = currentSqrDistance;
                            target = enemies[i];
                        }
                    }
                }
                break;
            case AimBehaviour.Farthest:
                {
                    float maxWalkDistance = 0f;
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if (InRange(enemies[i]) && enemies[i].GetComponent<Enemy>().distanceWalked >= maxWalkDistance)
                        {
                            maxWalkDistance = enemies[i].GetComponent<Enemy>().distanceWalked;
                            target = enemies[i];
                        }
                    }
                }
                break;
            case AimBehaviour.Healthiest:
                {
                    int maxHealth = 0;
                    for (int i = 0; i < enemies.Length; i++)
                    {
                        if (InRange(enemies[i]) && enemies[i].GetComponent<Enemy>().health >= maxHealth)
                        {
                            maxHealth = enemies[i].GetComponent<Enemy>().health;
                            target = enemies[i];
                        }
                    }
                }
                break;
            default: break;
        }
        if (target) return target.GetComponent<Enemy>();
        else return null;
    }
    protected bool InRange(GameObject target)
    {
        return (target.transform.position - transform.position).sqrMagnitude <= range * range;
    }
    public bool CanUpgrade()
    {
        if(NextUpgradeExists())
        {
            if(GameManager.Player.CanBuy(GetNextUpgrade().cost))
            {
                return true;
            }
            else
            {
                Debug.Log("CanUpgrade: Insufficient funds.");
                return false;
            }
        }
        else
        {
            Debug.Log("CanUpgrade: No next upgrade");
            return false;
        }
    }
    public bool NextUpgradeExists()
    {
        return GameManager.TowerManager.tower[data.type].Count > data.upgradeLevel + 1;
    }
    public Tower GetNextUpgrade()
    {
        return GameManager.TowerManager.tower[data.type][data.upgradeLevel + 1].prefab.GetComponent<Tower>();
    }
    public Tower Upgrade()
    {
        if (!CanUpgrade()) return null;
        Tower nextTower = GetNextUpgrade();
        GameManager.Player.Buy(nextTower.cost);

        nextTower = Instantiate(nextTower.gameObject).GetComponent<Tower>();
        nextTower.transform.position = transform.position;
        nextTower.transform.rotation = transform.rotation;
        nextTower.data.position = data.position;

        GameManager.LevelManager.DetachTowerFromTile(this);
        GameManager.LevelManager.AddTowerOnTile(data.position, nextTower);

        Debug.Log("Tower " + name + " has been upgraded!");
        return nextTower;
    }
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, range);
        for (int i = 0; i < muzzleEnd.Length; i++)
            Gizmos.DrawRay(muzzleEnd[i].transform.position, muzzleEnd[i].transform.forward * 3);
    }
}