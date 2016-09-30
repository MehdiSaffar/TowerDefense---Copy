using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class BrickSelectionUIScript : MonoBehaviour
{
    public delegate void OnTowerBuyClick(TowerType type);

    public event OnTowerBuyClick TowerBuyClick;

    public void TriggerBuyClick(TowerType towerType)
    {
        if (TowerBuyClick != null)
        {
            TowerBuyClick(towerType);
        }
    }

    public GameObject towers;
    public float radius;
    public Vector3 itemCostOffset;

    void Start()
    {
        InitializeTowers();
    }

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

    private void InitializeTowers()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        int index = 0;
        float startAngle = 0f;
        float angleIncrement = 45f;
        float angle = startAngle;
        foreach (var pair in GameManager.TowerManager.tower)
        {
            index++;
            angle += angleIncrement;

            Tower tower = pair.Value[0].prefab.GetComponent<Tower>();
            Sprite _icon = pair.Value[0].icon;
            string name = tower.name;
            int cost = tower.cost;

            GameObject iconGO = new GameObject(name);
            iconGO.transform.SetParent(towers.transform);
            iconGO.transform.localPosition = radius * (Vector3.up * Mathf.Sin(angle) + Vector3.right * Mathf.Cos(angle));
            iconGO.AddComponent<Image>().sprite = _icon;
            iconGO.AddComponent<Button>().onClick.AddListener(delegate { TriggerBuyClick(tower.data.type); });

            GameObject itemCostGO = Instantiate(GUIManager.ItemCost.gameObject);
            itemCostGO.SetActive(true);
            itemCostGO.transform.localScale /= 2f;
            itemCostGO.name = name + " cost";
            itemCostGO.transform.SetParent(iconGO.transform);
            itemCostGO.transform.localPosition = itemCostOffset; 
            itemCostGO.GetComponent<ItemCostUIScript>().Cost = cost;
        }
    }
}
