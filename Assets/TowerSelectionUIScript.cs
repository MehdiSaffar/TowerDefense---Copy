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
        SetAimBehaviour((Tower.AimBehaviour) aimBehaviour);
    }

    [SerializeField]
    private Button upgradeButton;
    [SerializeField]
    private ItemCostUIScript upgradeCost;
    [SerializeField]
    private ItemCostUIScript sellCost;

    public Image closest;
    public Image farthest;
    public Image healthiest;

    private Tower currentTower;
    public Color unselectedColor;
    public Color selectedColor;

    private void SetAimBehaviour(Tower.AimBehaviour behaviour)
    {
        closest.color = unselectedColor;
        farthest.color = unselectedColor;
        healthiest.color = unselectedColor;

        switch (behaviour)
        {
            case Tower.AimBehaviour.Closest:
                closest.color = selectedColor;
                break;
            case Tower.AimBehaviour.Farthest:
                farthest.color = selectedColor;
                break;
            case Tower.AimBehaviour.Healthiest:
                healthiest.color = selectedColor;
                break;
        }
    }
	public void Show(Tower tower)
    {
        if(GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        currentTower = tower;
        sellCost.Cost = tower.GetSellPrice();
        CheckUpgrade(GameManager.Player.Money);
        SetAimBehaviour(tower.aimBehaviour);

        EventManager.MoneyUpdate += CheckUpgrade;
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        currentTower = null;

        EventManager.MoneyUpdate -= CheckUpgrade;
        gameObject.SetActive(false);
    }
    public void CheckUpgrade(int newMoney)
    {
        bool active = currentTower && currentTower.NextUpgradeExists();
        upgradeButton.gameObject.SetActive(active);
        if(active)
        {
            upgradeCost.Show();
            upgradeCost.Cost = currentTower.GetNextUpgrade().cost;
        }
        else
        {
            upgradeCost.Hide();
        }
    }


}
