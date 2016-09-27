using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TowerPropertiesUIScript : MonoBehaviour {
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
        if(UpgradeClick != null) UpgradeClick();
    }
    public void TriggerAimBehaviourChange(int aimBehaviour)
    {
        if (AimBehaviourClick != null) AimBehaviourClick((Tower.AimBehaviour)aimBehaviour);
        SetAimBehaviourButtons((Tower.AimBehaviour)aimBehaviour);
    }

    public Image towerIconImage;
    public Button upgradeButton;

    public Text sellButtonText;

    [Header("Aim behaviour")]
    public Image closestButtonImage;
    public Image farthestButtonImage;
    public Image healthiestButtonImage;
    public Color activeAimBehaviourButtonColor;
    public Color inactiveAimBehaviourButtonColor;

    public void ShowTowerProperties(Tower tower)
    {
        upgradeButton.gameObject.SetActive(tower.NextUpgradeExists());
        towerIconImage.sprite = GameManager.TowerManager.tower[tower.data.type][tower.data.upgradeLevel].icon;
        sellButtonText.text = "Sell for $" + tower.GetSellPrice(); 
        SetAimBehaviourButtons(tower.aimBehaviour);
    }
    private void SetAimBehaviourButtons(Tower.AimBehaviour aimBehaviour)
    {
        closestButtonImage.color = inactiveAimBehaviourButtonColor;
        farthestButtonImage.color = inactiveAimBehaviourButtonColor;
        healthiestButtonImage.color = inactiveAimBehaviourButtonColor;
        switch (aimBehaviour)
        {
            case Tower.AimBehaviour.Closest:
                closestButtonImage.color = activeAimBehaviourButtonColor;
                break;
            case Tower.AimBehaviour.Farthest:
                farthestButtonImage.color = activeAimBehaviourButtonColor;
                break;
            case Tower.AimBehaviour.Healthiest:
                healthiestButtonImage.color = activeAimBehaviourButtonColor;
                break;
        }
    }
}
