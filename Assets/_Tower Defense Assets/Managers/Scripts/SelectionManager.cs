using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SelectionManager : MonoBehaviour
{
    [HideInInspector]
    public SelectionManager instance = null;

#pragma warning disable 0649
    public UI.Selection UIScript;
#pragma warning restore 0649

    [Header("Sound effects")]
    public AudioClip onPlaceNewTower;
    public AudioClip onError;
    public AudioClip onUpgrade;

    private GameObject clickedDown;
    private GameObject clickedUp;

    [Header("For Debug ONLY")]
    [SerializeField] private Tower selectedTower;
    [SerializeField] private Brick selectedBrick;

    private void SetHighlightRecursively(GameObject obj, bool highlight)
    {
        Highlightable highlightable = obj.GetComponent<Highlightable>();
        if(highlightable)
        {
            if(highlight)
            {
                Camera.main.GetComponent<HighlightPostProcess>().AddObject(highlightable);
            }
            else
            {
                Camera.main.GetComponent<HighlightPostProcess>().RemoveObject(highlightable);
            }
        }
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            SetHighlightRecursively(obj.transform.GetChild(i).gameObject, highlight);
        }
    }
    /// <summary>
    /// Highlights the tower, shows its gizmo, and sets the tower as selected in the UIScript
    /// </summary>
    private void Select(Tower tower)
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        DeselectAll();

        SetHighlightRecursively(tower.gameObject, true);

        tower.rangeGizmo.SetActive(true);
        UIScript.SetTower(tower);

        Camera.main.GetComponent<CameraController>().FlyToObject(tower.gameObject);

        selectedTower = tower;
    }
    private void Deselect(Tower tower)
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        SetHighlightRecursively(tower.gameObject, false);

        tower.rangeGizmo.SetActive(false);
        UIScript.Deselect();
    }
    /// <summary>
    /// Sets the brick as selected in the UIScript
    /// </summary>
    private void Select(Brick brick)
    {
        DeselectAll();
        UIScript.SetBrick(brick);
        Camera.main.GetComponent<CameraController>().FlyToObject(brick.gameObject);

        selectedBrick = brick;
    }
    private void Deselect(Brick brick)
    {
        UIScript.Deselect();
    }
    /// <summary>
    /// Deselects all objects
    /// </summary>
    private void DeselectAll()
    {
        if (selectedTower)
        {
            Deselect(selectedTower);
        }
        if(selectedBrick)
        {
            Deselect(selectedBrick);
        }
        selectedBrick = null;
        selectedTower = null;
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

        UIScript.TowerBuyClick += OnTowerBuyClick;
        UIScript.SellClick += OnTowerSellClick;
        UIScript.UpgradeClick += OnTowerUpgradeClick;
        UIScript.AimBehaviourClick += OnAimBehaviourClick;
    }
    private void Reset()
    {
        // FIXME: 
        if (selectedTower != null)
        {
            Deselect(selectedTower);
            Destroy(selectedTower.gameObject);
            selectedTower = null;
        }
    }
    private void Fsm_Changed(GameManager.States state)
    {
        enabled = state == GameManager.States.Edit || state == GameManager.States.Play;
    }

    //--------- Events ----------//
    private void OnAimBehaviourClick(Tower.AimBehaviour aimBehaviour)
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        selectedTower.aimBehaviour = aimBehaviour;
    }
    private void OnTowerBuyClick(Vector2 pos, TowerType towerType)
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

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
        Tower newTower = Instantiate(GameManager.TowerManager.tower[towerType][0].prefab).GetComponent<Tower>();
        GameManager.LevelManager.AddTowerOnTile(pos, newTower);
        Select(newTower);
    }
    private void OnTowerSellClick()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        GameManager.Player.Money += selectedTower.GetSellPrice();
        GameManager.LevelManager.DetachTowerFromTile(selectedTower);

        Destroy(selectedTower.gameObject);
        DeselectAll();
    }
    private void OnTowerUpgradeClick()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }

        Tower oldTower = selectedTower;
        Tower upgradedTower = selectedTower.Upgrade();
        if (!upgradedTower)
        {
            SoundManager.RandomizeFx(onError);
            return;
        }
        SoundManager.RandomizeFx(onUpgrade);

        Select(upgradedTower);
        selectedTower = upgradedTower;

        Destroy(oldTower.gameObject);
    }
    private void OnObjectClick(Tower tower)
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }
        Select(tower);
        selectedTower = tower;
    }
    private void OnObjectClick(Brick brick)
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }
        if (GameManager.LevelManager.IsAvailable(brick.internalPos))
        {
            Select(brick);
            selectedBrick = brick;
        }
        else
        {
            OnVoidClick();
        }
    }
    private void OnVoidClick()
    {
        if (GameManager.ShowDebug)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
        }
        DeselectAll();
        selectedTower = null;
        selectedBrick = null;
    }

    public void Update()
    {
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

                // If the user clicks on something
                if (clickedDown == clickedUp)
                {

                    if (selectedTower)
                    {
                        if (tower && selectedTower == tower)
                        {
                            OnVoidClick();
                            return;
                        }
                        else if (tower)
                        {
                            OnObjectClick(tower);
                        }
                        else
                        {
                            OnVoidClick();
                        }
                    }
                    else if (selectedBrick)
                    {
                        if (tower)
                        {
                            OnObjectClick(tower);
                        }
                        else
                        {
                            OnVoidClick();
                        }
                    }
                    else
                    {
                        if(brick)
                        {
                            OnObjectClick(brick);
                        }
                        else if(tower)
                        {
                            OnObjectClick(tower);
                        }
                        else
                        {
                            OnVoidClick();
                        }
                    }
                }
            }
        }
    }
}
