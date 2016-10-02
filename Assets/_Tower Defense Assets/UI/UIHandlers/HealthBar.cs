using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : UIElement
    {
        [SerializeField]
        private Slider healthSlider;
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
}
