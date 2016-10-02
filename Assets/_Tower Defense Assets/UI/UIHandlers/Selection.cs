using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Selection : UIElement
    {
        public delegate void OnUpgradeClick();
        public delegate void OnSellClick();
        public delegate void OnTowerBuyClick(Vector2 pos, TowerType type);
        public delegate void OnAimBehaviourClick(Tower.AimBehaviour aimBehaviour);

        public event OnUpgradeClick UpgradeClick;
        public event OnTowerBuyClick TowerBuyClick;
        public event OnSellClick SellClick;
        public event OnAimBehaviourClick AimBehaviourClick;

        public void TriggerBuyClick(TowerType towerType)
        {
            if (TowerBuyClick != null)
            {
                Brick brick = currentObject.GetComponent<Brick>();
                if (brick)
                {
                    TowerBuyClick(brick.internalPos, towerType);
                }
            }
        }
        public void TriggerSellClick()
        {
            if (SellClick != null) SellClick();
        }
        public void TriggerUpgradeClick()
        {
            if (UpgradeClick != null) UpgradeClick();
        }
        public void TriggerAimBehaviourChange(Tower.AimBehaviour aimBehaviour)
        {
            if (AimBehaviourClick != null) AimBehaviourClick(aimBehaviour);
        }

        [Header("UI Objects")]
        [SerializeField]
        private TowerSelection towerSelectionUIScript;
        [SerializeField]
        private BrickSelection brickSelectionUIScript;

        [Header("For Debug ONLY")]
        [SerializeField]
        private GameObject currentObject;
        [SerializeField]
        private float objectScale = 1.0f;
        [SerializeField]
        private Vector3 initialScale;
        [SerializeField]
        private float lowestScale;
        [SerializeField]
        private float highestScale;

        void Awake()
        {
            initialScale = transform.localScale;
        }
        void Start()
        {
            towerSelectionUIScript.AimBehaviourClick += TriggerAimBehaviourChange;
            towerSelectionUIScript.SellClick += TriggerSellClick;
            towerSelectionUIScript.UpgradeClick += TriggerUpgradeClick;

            brickSelectionUIScript.TowerBuyClick += TriggerBuyClick;
        }
        void OnDestroy()
        {
            towerSelectionUIScript.AimBehaviourClick -= TriggerAimBehaviourChange;
            towerSelectionUIScript.SellClick -= TriggerSellClick;
            towerSelectionUIScript.UpgradeClick -= TriggerUpgradeClick;

            brickSelectionUIScript.TowerBuyClick -= TriggerBuyClick;
        }

        public void SetTower(Tower tower)
        {
            if (GameManager.ShowDebug)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
            }

            currentObject = tower.gameObject;
            brickSelectionUIScript.Hide();
            towerSelectionUIScript.Show(tower);
        }
        public void SetBrick(Brick brick)
        {
            if (GameManager.ShowDebug)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
            }

            if (GameManager.LevelManager.IsAvailable(brick.internalPos))
            {
                currentObject = brick.gameObject;
                towerSelectionUIScript.Hide();
                brickSelectionUIScript.Show();
            }
        }
        public void Deselect()
        {
            if (GameManager.ShowDebug)
            {
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
                Debug.Log(Time.frameCount + " " + trace.GetFrame(0).GetMethod().Name);
            }

            currentObject = null;
            towerSelectionUIScript.Hide();
            brickSelectionUIScript.Hide();
        }

        void Update()
        {
            if (!currentObject) return;
            CenterUIOnObject();
        }
        private void CenterUIOnObject()
        {
            float dist = (Camera.main.transform.position - currentObject.transform.position).magnitude;
            transform.localScale = initialScale * Mathf.Max(1f / dist * objectScale, lowestScale);
            foreach (var render in GetComponentsInChildren<Renderer>())
                render.enabled = currentObject.GetComponentInChildren<Renderer>().isVisible;
            transform.position = Camera.main.WorldToScreenPoint(currentObject.transform.position);
        }
    }
}