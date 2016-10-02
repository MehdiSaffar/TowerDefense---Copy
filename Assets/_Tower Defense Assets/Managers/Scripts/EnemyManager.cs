using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public enum EnemyType {
    ENEMY_1,
    ENEMY_2,
    ENEMY_3,
}

public class EnemyManager : MonoBehaviour {
    [HideInInspector]
    public static EnemyManager instance = null;

    [Header("Enemy prefabs")]
    public GameObject Enemy1;
    public GameObject Enemy2;
    public GameObject Enemy3;

    public Dictionary<EnemyType, GameObject> enemyGO = new Dictionary<EnemyType, GameObject>();

    void Awake() {
        // Making sure there is only one instance of EnemyManager
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        // Making sure the EnemyManager persists across scene loads
        if(transform.root == transform) DontDestroyOnLoad(gameObject);

        if (Enemy1 != null) enemyGO.Add(EnemyType.ENEMY_1, Enemy1);
        if (Enemy2 != null) enemyGO.Add(EnemyType.ENEMY_2, Enemy2);
        if (Enemy3 != null) enemyGO.Add(EnemyType.ENEMY_3, Enemy3);
    }
    public static Dictionary<EnemyType, GameObject> EnemyGO {
        get { return instance.enemyGO; }
    }

}
