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
        if(GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        gameObject.SetActive(true);
    }
    public void Hide()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        gameObject.SetActive(false);
    }
}
