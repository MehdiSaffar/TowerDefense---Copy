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
    [SerializeField]
    private ItemCostUIScript upgradeCost;
    [SerializeField]
    private ItemCostUIScript sellCost;

    private Tower currentTower;

	public void Show(Tower tower)
    {
        if(GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        currentTower = tower;
        EventManager.MoneyUpdate += OnMoneyUpdate;
        gameObject.SetActive(true);
        OnMoneyUpdate(GameManager.Player.Money); // Check Update stuff
        sellCost.Cost = tower.GetSellPrice();
    }
    public void Hide()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        gameObject.SetActive(false);

        EventManager.MoneyUpdate -= OnMoneyUpdate;

        currentTower = null;
    }
    public void OnMoneyUpdate(int newMoney)
    {
        bool active = currentTower && currentTower.NextUpgradeExists();
        upgradeButton.gameObject.SetActive(active);
        if(active)
        {
            upgradeCost.Show();
            upgradeCost.Cost = currentTower.GetNextUpgrade().cost;
            if(GameManager.Player.CanBuy(currentTower.GetNextUpgrade().cost))
            {
                upgradeCost.ItemState = ItemCostUIScript.State.Available;
            }
            else
            {
                upgradeCost.ItemState = ItemCostUIScript.State.InsufficientFunds;
            }
        }
        else
        {
            upgradeCost.Hide();
        }
    }


}
