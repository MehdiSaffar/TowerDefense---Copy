using UnityEngine;

public class BrickPropertiesUIScript : MonoBehaviour
{
    private Brick brick;
    public Brick Brick
    {
        get
        {
            return brick;
        }
        set
        {
            brick = value;
        }
    }

    void Show(Brick brick)
    {
        Brick = brick;
    }
}