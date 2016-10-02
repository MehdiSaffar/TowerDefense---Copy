using UnityEngine;
using System.Collections;
namespace UI
{
    public class MainMenu : UIElement
    {
        public delegate void OnPlayClick();
        public event OnPlayClick PlayClick;

        public void TriggerPlayClick()
        {
            if(PlayClick != null) PlayClick();
        }
    }
}