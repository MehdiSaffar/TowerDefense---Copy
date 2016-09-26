using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TowerSelectionUIScript : MonoBehaviour {

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
    }

    [SerializeField]
    private Button upgradeButton;


	public void Show(Tower tower)
    {
        gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(tower.CanUpgrade());
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
