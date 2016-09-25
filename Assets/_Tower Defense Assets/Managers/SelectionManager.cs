using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SelectionManager : MonoBehaviour
{
    [HideInInspector] public SelectionManager instance = null;
    private SelectionUIScript UIScript;

    public enum SelectionMode
    {
        Normal,
        Moving,
        PlacingNewTower,
    };

    public SelectionMode mode = SelectionMode.Normal;

    [Header("Sound effects")]
    public AudioClip onPlaceNewTower;
    public AudioClip onError;
    public AudioClip onUpgrade;

    private Tower selectedTower;
    private Brick selectedBrick;
    private GameObject lastClickedDown;
    private GameObject clickedUp;

    private Tower SelectedTower
    {
        get
        {
            return selectedTower;
        }
        set
        {
            if (selectedTower != null && value == null)
            {
                Deselect(selectedTower);
                selectedTower = value;
            }
            else if (selectedTower != null && value != null && value != selectedTower)
            {
                Deselect(selectedTower);
                if(mode == SelectionMode.PlacingNewTower)
                {
                    Destroy(selectedTower.gameObject);
                }
                Select(value);
                selectedTower = value;
            }
            else if (selectedTower == null && value != null)
            {
                selectedTower = value;
                if(mode == SelectionMode.PlacingNewTower) selectedTower.enabled = false;
                Select(selectedTower);
            }
        }
    }
    private Brick SelectedBrick
    {
        get
        {
            return selectedBrick;
        }
        set
        {
            selectedBrick = value;
        }
    }
    private void Select(Tower tower)
    {
        foreach (var obj in tower.GetComponentsInChildren<Highlightable>())
            Camera.main.GetComponent<HighlightPostProcess>().AddObject(obj);
        tower.rangeGizmo.SetActive(true);
        UIScript.gameObject.SetActive(true);
        UIScript.SetTower(tower);
    }
    private void Deselect(Tower tower)
    {
        foreach (var obj in tower.GetComponentsInChildren<Highlightable>())
            Camera.main.GetComponent<HighlightPostProcess>().RemoveObject(obj);
        tower.rangeGizmo.SetActive(false);
        UIScript.gameObject.SetActive(false);
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("You cannot have two selection managers!");
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GameManager.Fsm.Changed += Fsm_Changed;
        GameManager.LevelManager.LevelLoaded += Reset;
        GUIManager.SelectionUI.TowerBuyClick += OnTowerBuyClick;
        GUIManager.SelectionUI.SellClick += OnTowerSellClick;
        GUIManager.SelectionUI.UpgradeClick += OnTowerUpgradeClick;

        UIScript = GUIManager.SelectionUI;
    }
    private void Reset()
    {
        if (selectedTower != null)
        {
            Deselect(selectedTower);
            Destroy(selectedTower.gameObject);
            selectedTower = null;
        }
        mode = SelectionMode.Normal;
    }
    private void Fsm_Changed(GameManager.States state)
    {
        enabled = state == GameManager.States.Edit || state == GameManager.States.Play;
    }
    //--------- Events ----------//
    public void OnAimBehaviourClick(Tower.AimBehaviour aimBehaviour)
    {
        SelectedTower.aimBehaviour = aimBehaviour;
    }
    public void OnTowerBuyClick(Vector2 pos, TowerType towerType)
    {
        int cost = GameManager.TowerManager.tower[towerType][0].prefab.GetComponent<Tower>().cost;
        if (!GameManager.Player.CanBuy(cost))
        {
            Debug.Log("Not enough funds to buy " + towerType.ToString());
            return;
        }
        if(!GameManager.LevelManager.IsAvailable(pos))
        {
            Debug.LogError("Cannot place " + towerType.ToString() + " on the brick!");
            return;
        }
        GameManager.Player.Buy(cost);
        SelectedTower = Instantiate(GameManager.TowerManager.tower[towerType][0].prefab).GetComponent<Tower>();
        GameManager.LevelManager.AddTowerOnTile(pos, SelectedTower);
        UIScript.Deselect();
    }
    public void OnTowerSellClick()
    {
        GameManager.Player.Money += SelectedTower.GetSellPrice();
        GameManager.LevelManager.DetachTowerFromTile(selectedTower);
        Deselect(selectedTower);
        Destroy(selectedTower.gameObject);
        selectedTower = null;
    }
    public void OnTowerUpgradeClick()
    {
        Tower upgradedTower = SelectedTower.GetNextUpgrade();
        if(upgradedTower != null)
        {
            GameManager.SoundManager.RandomizeFx(onUpgrade);
            mode = SelectionMode.Normal;
            Tower oldTower = SelectedTower;
            SelectedTower = upgradedTower;
            Destroy(oldTower.gameObject);
        }
        else
        {
            GameManager.SoundManager.RandomizeFx(onError);
        }
    }
    private void OnEmptyTileClick(Brick brick)
    {
        SelectedBrick = brick;
        UIScript.gameObject.SetActive(true);
        UIScript.SetBrick(brick);
    }
    private void OnTowerClick(Tower tower)
    {
        SelectedTower = tower;
        UIScript.gameObject.SetActive(true);
        UIScript.SetTower(tower);
    }
    private void OnBrickClick(Brick selectedBrick)
    {
        SelectedBrick = selectedBrick;
        UIScript.gameObject.SetActive(true);
        UIScript.SetBrick(SelectedBrick);
    }
    private void OnVoidClick()
    {
        SelectedTower = null;
        SelectedBrick = null;
        UIScript.gameObject.SetActive(false);
    }

    public void Update()
    {
        // To prevent clicking through the UI
        if (EventSystem.current.IsPointerOverGameObject()) return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay((Input.touchCount == 1) ? Input.touches[0].position : (Vector2)Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Tower tower = hit.collider.GetComponent<Tower>();
            Brick brick = hit.collider.GetComponent<Brick>();
            if (Input.GetMouseButtonDown(0))
            {
                lastClickedDown = hit.collider.gameObject;
            }
            else if (Input.GetMouseButton(0))
            {
            }
            else if (Input.GetMouseButtonUp(0))
            {
                clickedUp = hit.collider.gameObject;

                // If we click on something that is nothing, we basically want to deselect
                if(!brick && !tower)
                {
                    OnVoidClick();
                    return;
                }
                switch (mode)
                {
                    case SelectionMode.Normal:
                        if (brick)
                        {
                            if(GameManager.LevelManager.IsAvailable(brick.internalPos))
                            {
                                OnEmptyTileClick(brick);
                            }
                        }

                        // If the user clicks on something different
                        if (SelectedTower)
                        {
                            if (clickedUp != SelectedTower.gameObject)
                            {
                                OnVoidClick();
                            }
                        }
                        if (SelectedBrick)
                        {
                            if(clickedUp != SelectedBrick.gameObject)
                            {
                                OnVoidClick();
                            }
                        }
                        // If the user clicks on the selected tower
                        if (tower && lastClickedDown == clickedUp)
                        {
                            SelectedTower = SelectedTower ? null : tower;
                            if (SelectedTower)
                            {
                                OnTowerClick(SelectedTower);
                            }
                        }
                        if (brick && lastClickedDown == clickedUp)
                        {
                            SelectedBrick = SelectedBrick ? null : brick;
                            if (SelectedBrick)
                            {
                                OnBrickClick(SelectedBrick);
                            }
                        }
                        break;
                }
            }
        }
    }
}
