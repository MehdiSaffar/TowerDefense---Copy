using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class WaveIndicator : UIElement
    {
        [SerializeField]
        private Text waveIndexText;
        private int waveIndex;
        public int WaveIndex
        {
            get
            {
                return waveIndex;
            }
            set
            {
                waveIndex = value;
                if (waveIndexText) waveIndexText.text = "Wave " + waveIndex.ToString();
            }
        }
        public void Awake()
        {
            WaveIndex = GameManager.LevelManager.waveSpawner.WaveIndex + 1;
            EventManager.WaveIndexUpdate += OnWaveIndexChange;
        }

        public void OnWaveIndexChange(int newWaveIndex)
        {
            WaveIndex = newWaveIndex + 1;
        }
    }
}