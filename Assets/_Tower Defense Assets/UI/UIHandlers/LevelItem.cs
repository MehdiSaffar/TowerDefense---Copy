using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelItem : UIElement
    {
        public Text levelName;
        public Image thumbnailImage;
        public Image lockedImage;

        private bool _locked;
        public bool locked
        {
            get
            {
                return _locked;
            }
            set
            {
                _locked = value;
                lockedImage.gameObject.SetActive(_locked);
            }
        }

        public void Set(string _name, Texture2D _thumbnail, bool _locked)
        {
            levelName.text = _name;
            if (_thumbnail) thumbnailImage.overrideSprite = Sprite.Create(_thumbnail, new Rect(0, 0, _thumbnail.width, _thumbnail.height), new Vector2(0, 0));
            locked = _locked;
        }
    }
}