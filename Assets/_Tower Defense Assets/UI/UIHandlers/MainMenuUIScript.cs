using UnityEngine;
using System.Collections;

public class MainMenuUIScript : MonoBehaviour {
    public delegate void OnPlayClick();
    public event OnPlayClick PlayClick;

    public void TriggerPlayClick()
    {
        PlayClick();
    }
}
