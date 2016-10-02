using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace UI
{
    public class Money : UIElement
    {
        private int dollarAmount;
        public Text dollarAmountText;

        public int DollarAmount
        {
            get
            {
                return dollarAmount;
            }
            set
            {
                dollarAmount = value;
                if (dollarAmountText) dollarAmountText.text = dollarAmount.ToString();
            }
        }
        public void Awake()
        {
            DollarAmount = GameManager.Player.Money;
            EventManager.MoneyUpdate += OnMoneyUpdate;
        }
        private void OnMoneyUpdate(int newMoney)
        {
            DollarAmount = newMoney;
        }
    }
}