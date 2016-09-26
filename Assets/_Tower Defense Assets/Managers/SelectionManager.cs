using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SelectionManager : MonoBehaviour
{
    [HideInInspector]
    public SelectionManager instance = null;
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
    private GameObject clickedDown;
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
                if (mode == SelectionMode.PlacingNewTower)
                {
                    Destroy(selectedTower.gameObject);
                }
                Select(value);
                selectedTower = value;
            }
            else if (selectedTower == null && value != null)
            {
                selectedTower = value;
                if (mode == SelectionMode.PlacingNewTower) selectedTower.enabled = false;
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
        UIScript.SetTower(tower);
    }
    private void Deselect(Tower tower)
    {
        foreach (var obj in tower.GetComponentsInChildren<Highlightable>())
            Camera.main.GetComponent<HighlightPostProcess>().RemoveObject(obj);
        tower.rangeGizmo.SetActive(false);
        UIScript.Deselect();
    }
    private void Select(Brick brick)
    {
        UIScript.SetBrick(brick);
    }
    private void Deselect(Brick brick)
    {
        UIScript.Deselect();
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

        UIScript = GUIManager.SelectionUI;
        UIScript.TowerBuyClick += OnTowerBuyClick;
        UIScript.SellClick += OnTowerSellClick;
        UIScript.UpgradeClick += OnTowerUpgradeClick;
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
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        SelectedTower.aimBehaviour = aimBehaviour;
    }
    public void OnTowerBuyClick(Vector2 pos, TowerType towerType)
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        int cost = GameManager.TowerManager.tower[towerType][0].prefab.GetComponent<Tower>().cost;
        if (!GameManager.Player.CanBuy(cost))
        {
            Debug.Log("Not enough funds to buy " + towerType.ToString());
            return;
        }
        if (!GameManager.LevelManager.IsAvailable(pos))
        {
            Debug.LogError("Cannot place " + towerType.ToString() + " on the brick!");
            return;
        }
        GameManager.Player.Buy(cost);
        selectedTower = Instantiate(GameManager.TowerManager.tower[towerType][0].prefab).GetComponent<Tower>();
        GameManager.LevelManager.AddTowerOnTile(pos, SelectedTower);

        Select(SelectedTower);
    }
    public void OnTowerSellClick()
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        GameManager.Player.Money += SelectedTower.GetSellPrice();
        GameManager.LevelManager.DetachTowerFromTile(selectedTower);

        Deselect(selectedTower);
        Destroy(selectedTower.gameObject);

        selectedTower = null;
        selectedBrick = null;

    }
    public void OnTowerUpgradeClick()
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        Tower upgradedTower = SelectedTower.GetNextUpgrade();
        if (!upgradedTower)
        {
            GameManager.SoundManager.RandomizeFx(onError);
            return;
        }

        GameManager.SoundManager.RandomizeFx(onUpgrade);
        Tower oldTower = SelectedTower;
        Select(upgradedTower);
        SelectedTower = upgradedTower;

        Destroy(oldTower.gameObject);
    }

    private void OnTowerClick(Tower tower)
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        if (tower == null) Debug.Log("Weird");
        Select(tower);
        SelectedTower = tower;
    }
    private void OnBrickClick(Brick brick)
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);

        if (brick == null) Debug.Log("Weird");
        Select(brick);
        selectedBrick = brick;
    }
    private void OnVoidClick()
    {
        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
        Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        if(selectedTower)
        {
            selectedTower = null;
            Deselect(SelectedTower);
        }
        if(selectedBrick)
        {
            selectedBrick = null;
            Deselect(SelectedBrick);
        }
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
                clickedDown = hit.collider.gameObject;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                clickedUp = hit.collider.gameObject;

                // If we click on something that is nothing, we basically want to deselect
                if (!(brick || tower))
                {
                    OnVoidClick();
                    return;
                }

                // If the user clicks on the selected tower
                if (tower && clickedDown == clickedUp)
                {
                    if(SelectedTower)
                    {
                        OnVoidClick();
                    }
                    else
                    {
                        OnTowerClick(tower);
                    }
                }
                // If the user clicks on the selected brick
                else if (brick && clickedDown == clickedUp)
                {
                    if (SelectedBrick)
                    {
                        OnVoidClick();
                    }
                    else
                    {
                        OnBrickClick(brick);
                    }
                }
                // If the user clicks on something different
                else if (SelectedTower && clickedUp != SelectedTower.gameObject)
                {
                    OnVoidClick();
                }
                else if (SelectedBrick && clickedUp != SelectedBrick.gameObject)
                {
                    OnVoidClick();
                }
            }
        }
    }
}
