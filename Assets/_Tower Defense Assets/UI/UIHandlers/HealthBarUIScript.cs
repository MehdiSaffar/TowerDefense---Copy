using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIScript : MonoBehaviour {
    [SerializeField] private Slider healthSlider;
    private int health;
    
    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            healthSlider.value = health;
        }
    }
    public void Start()
    {
        EventManager.HealthUpdate += OnHealthUpdate;
    }

    private void OnHealthUpdate(int newHealth)
    {
        Health = newHealth;
    }
}
