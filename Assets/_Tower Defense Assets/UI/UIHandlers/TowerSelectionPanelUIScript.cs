using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowerSelectionPanelUIScript : MonoBehaviour {
    public delegate void OnTowerClick(TowerType tower);
    public event OnTowerClick TowerClick;

    public void TriggerTowerClick(int tower) {
        TowerClick((TowerType)tower);
    }
}
