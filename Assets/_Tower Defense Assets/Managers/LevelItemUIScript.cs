using UnityEngine;
using UnityEngine.UI;

public class LevelItemUIScript : MonoBehaviour
{
    public Text levelName;
    public Image thumbnailImage;
    public Image lockedImage;
    private bool locked;
    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
            locked = value;
            lockedImage.gameObject.SetActive(locked);
        }
    }

    public void Set(string _name, Texture2D _thumbnail, bool _locked)
    {
        levelName.text = _name;
        if(_thumbnail) thumbnailImage.overrideSprite = Sprite.Create(_thumbnail, new Rect(0,0, _thumbnail.width, _thumbnail.height), new Vector2(0,0));
        Locked = _locked;
    }

}