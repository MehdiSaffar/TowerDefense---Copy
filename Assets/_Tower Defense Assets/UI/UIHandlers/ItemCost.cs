using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
    public class ItemCost : UIElement
    {
        public enum State
        {
            Available,
            InsufficientFunds,
        };

        [Header("Colors by state")]
        public Color availableColor;
        public Color insufficientFundsColor;

        [SerializeField]
        private Text costText;

        private int cost;
        public int Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
                costText.text = "$" + cost.ToString();
                OnMoneyUpdate(GameManager.Player.Money);
            }
        }

        private State itemState = State.Available;
        public State ItemState
        {
            get
            {
                return itemState;
            }
            set
            {
                itemState = value;
                switch (itemState)
                {
                    case State.Available:
                        GetComponent<Image>().color = availableColor;
                        break;
                    case State.InsufficientFunds:
                        GetComponent<Image>().color = insufficientFundsColor;
                        break;
                }
            }
        }
        public void Show()
        {
            EventManager.MoneyUpdate += OnMoneyUpdate;
            gameObject.SetActive(true);
        }
        private void OnMoneyUpdate(int newMoney)
        {
            if (newMoney < cost)
            {
                ItemState = State.InsufficientFunds;
            }
            else
            {
                ItemState = State.Available;
            }
        }
        public void Hide()
        {
            EventManager.MoneyUpdate -= OnMoneyUpdate;
            gameObject.SetActive(false);
        }
    }
}