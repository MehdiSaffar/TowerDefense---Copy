using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public enum TowerType {
    TURRET_1 = 1,
    TURRET_2,
    BOMBER_1,
}

[Serializable]
public class TowerProperties
{
    public GameObject prefab;
    public Sprite icon;
}

public class TowerManager : MonoBehaviour {
    [HideInInspector] public static TowerManager instance = null;

    [Header("Tower prefabs")]
    public List<TowerProperties> Turret1;
    public List<TowerProperties> Turret2;
    public List<TowerProperties> Bomber1;

    public float sellPercentage = 0.8f;

    public Dictionary<TowerType, List<TowerProperties>> tower = new Dictionary<TowerType, List<TowerProperties>>();
    void Awake() {
        // Making sure there is only one instance of TowerManager
        if (instance == null)
        {
            instance = this;
            if(transform.root == transform) DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        tower.Add(TowerType.TURRET_1, Turret1);
        tower.Add(TowerType.TURRET_2, Turret2);
        tower.Add(TowerType.BOMBER_1, Bomber1);
    }
}
