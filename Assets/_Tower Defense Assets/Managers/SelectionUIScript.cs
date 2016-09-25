using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionUIScript : MonoBehaviour {
    public delegate void OnUpgradeClick();
    public delegate void OnSellClick();
    public delegate void OnAimBehaviourClick(Tower.AimBehaviour aimBehaviour);

    public event OnUpgradeClick UpgradeClick;
    public event OnSellClick SellClick;
    public event OnAimBehaviourClick AimBehaviourClick;

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
        SetAimBehaviourButtons((Tower.AimBehaviour)aimBehaviour);
    }

    private void SetAimBehaviourButtons(Tower.AimBehaviour aimBehaviour)
    {
        throw new NotImplementedException();
    }

    public enum UIMode
    {
        Tower,
        Brick,
    };

    [Header("UI Objects")]
    [SerializeField] private Text text;

    [Header("For Debug ONLY")]
    [SerializeField] private UIMode mode;
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
        if(tower)
        {
            currentObject = tower.gameObject;
            mode = UIMode.Tower;
            text.text = "Tower selected!";
            CenterUIOnObject(currentObject);

        }
    }
    public void SetBrick(Brick brick)
    {
        if(brick)
        {
            currentObject = brick.gameObject;
            mode = UIMode.Brick;
            text.text = "Brick selected!";
            CenterUIOnObject(currentObject);
        }
    }
    void LateUpdate()
    {
        if (!currentObject) return;
        CenterUIOnObject(currentObject);
    }
    void CenterUIOnObject(GameObject obj)
    {
        float dist = (Camera.main.transform.position - obj.transform.position).magnitude;
        transform.localScale = initialScale * 1f/dist * objectScale;
        foreach (var render in GetComponentsInChildren<Renderer>())
        {
            render.enabled = obj.GetComponentInChildren<Renderer>().isVisible;
        }
        transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
    }
}