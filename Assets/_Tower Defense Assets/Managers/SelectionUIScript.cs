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

    public void TriggerBuyClick(TowerType towerType)
    {
        if (TowerBuyClick != null)
        {
            Brick brick = currentObject.GetComponent<Brick>();
            if (brick)
            {
                TowerBuyClick(brick.internalPos, towerType);
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
    public void TriggerAimBehaviourChange(Tower.AimBehaviour aimBehaviour)
    {
        if (AimBehaviourClick != null) AimBehaviourClick(aimBehaviour);
    }

    [Header("UI Objects")]
    [SerializeField] private TowerSelectionUIScript towerSelectionUIScript;
    [SerializeField] private BrickSelectionUIScript brickSelectionUIScript;

    [Header("For Debug ONLY")]
    [SerializeField] private GameObject currentObject;
    [SerializeField] private float objectScale = 1.0f;
    [SerializeField] private Vector3 initialScale;

    void Awake()
    {
        initialScale = transform.localScale;
    }
    void Start()
    {
        towerSelectionUIScript.AimBehaviourClick += TriggerAimBehaviourChange;
        towerSelectionUIScript.SellClick += TriggerSellClick;
        towerSelectionUIScript.UpgradeClick += TriggerUpgradeClick;

        brickSelectionUIScript.TowerBuyClick += TriggerBuyClick;
    }
    void OnDestroy()
    {
        towerSelectionUIScript.AimBehaviourClick -= TriggerAimBehaviourChange;
        towerSelectionUIScript.SellClick -= TriggerSellClick;
        towerSelectionUIScript.UpgradeClick -= TriggerUpgradeClick;

        brickSelectionUIScript.TowerBuyClick -= TriggerBuyClick;
    }

    public void SetTower(Tower tower)
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        currentObject = tower.gameObject;
        brickSelectionUIScript.Hide();
        towerSelectionUIScript.Show(tower);
    }
    public void SetBrick(Brick brick)
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        if (GameManager.LevelManager.IsAvailable(brick.internalPos))
        {
            currentObject = brick.gameObject;
            towerSelectionUIScript.Hide();
            brickSelectionUIScript.Show();
        }
    }
    public void Deselect()
    {
        currentObject = null;
        towerSelectionUIScript.Hide();
        brickSelectionUIScript.Hide();
    }

    void LateUpdate()
    {
        if (!currentObject) return;
        CenterUIOnObject();
    }
    private void CenterUIOnObject()
    {
        float dist = (Camera.main.transform.position - currentObject.transform.position).magnitude;
        transform.localScale = initialScale * 1f/dist * objectScale;
        foreach (var render in GetComponentsInChildren<Renderer>())
            render.enabled = currentObject.GetComponentInChildren<Renderer>().isVisible;
        transform.position = Camera.main.WorldToScreenPoint(currentObject.transform.position);
    }

}