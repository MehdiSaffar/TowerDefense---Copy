using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUIScript : MonoBehaviour {
    public delegate void OnUpgradeClick();
    public delegate void OnSellClick();
    public delegate void OnTowerBuyClick(Vector2 pos, TowerType type);
    public delegate void OnAimBehaviourClick(Tower.AimBehaviour aimBehaviour);

    public event OnUpgradeClick UpgradeClick;
    public event OnTowerBuyClick TowerBuyClick;
    public event OnSellClick SellClick;
    public event OnAimBehaviourClick AimBehaviourClick;

    public void TriggerBuyClick(int towerType)
    {
        if (TowerBuyClick != null)
        {
            Brick brick = currentObject.GetComponent<Brick>();
            if(brick)
            {
                TowerBuyClick(brick.internalPos, (TowerType)towerType);
            }
        }
    }
    public void TriggerSellClick()
    {
        if (SellClick != null) SellClick();
    }
    public void TriggerUpgradeClick()
    {
        if (UpgradeClick != null) UpgradeClick();
    }
    public void TriggerAimBehaviourChange(int aimBehaviour)
    {
        if (AimBehaviourClick != null) AimBehaviourClick((Tower.AimBehaviour)aimBehaviour);
    }

    [Header("UI Objects")]
    [SerializeField] private Button sellButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private GameObject towers;

    [Header("For Debug ONLY")]
    [SerializeField] private GameObject currentObject;
    public Camera cam;
    public float objectScale = 1.0f;

    [SerializeField] private Vector3 initialScale;

    void Awake()
    {
        initialScale = transform.localScale;
    }
    public void SetTower(Tower tower)
    {
        if (tower)
        {
            if (currentObject)
            {
                Deselect();
                return;
            }
            currentObject = tower.gameObject;
            SetActiveRecursively(sellButton.gameObject, true);
            SetActiveRecursively(upgradeButton.gameObject, true);

            CenterUIOnObject();
        }
    }
    public void SetBrick(Brick brick)
    {
        if(brick)
        {
            if (currentObject)
            {
                Deselect();
                return;
            }
            currentObject = brick.gameObject;
            SetActiveRecursively(towers, true);
            CenterUIOnObject();
        }
    }


    void LateUpdate()
    {
        if (!currentObject) return;
        CenterUIOnObject();
    }
    void CenterUIOnObject()
    {
        float dist = (Camera.main.transform.position - currentObject.transform.position).magnitude;
        transform.localScale = initialScale * 1f/dist * objectScale;
        foreach (var render in GetComponentsInChildren<Renderer>())
            render.enabled = currentObject.GetComponentInChildren<Renderer>().isVisible;
        transform.position = Camera.main.WorldToScreenPoint(currentObject.transform.position);
    }

    public void Deselect()
    {
        currentObject = null;
        SetActiveRecursively(gameObject, false);
    }

    private void SetActiveRecursively(GameObject obj, bool active)
    {
        obj.SetActive(active);
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetActiveRecursively(obj.transform.GetChild(i).gameObject, active);
        }
    }
}