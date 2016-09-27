using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemCostUIScript : MonoBehaviour {
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
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
