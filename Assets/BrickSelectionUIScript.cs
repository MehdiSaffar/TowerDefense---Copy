using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BrickSelectionUIScript : MonoBehaviour
{
    public delegate void OnTowerBuyClick(TowerType type);

    public event OnTowerBuyClick TowerBuyClick;

    public void TriggerBuyClick(int towerType)
    {
        if (TowerBuyClick != null)
        {
            TowerBuyClick((TowerType)towerType);
        }
    }

    [SerializeField]
    private GameObject towers;

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
