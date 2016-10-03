using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

public class GUIManager : MonoBehaviour
{
    [HideInInspector] public static GUIManager instance = null;
    public enum UILayer
    {
        InGame = 0,
        HUD,
        Menu,
    };

    public static List<Transform> layers;

    public void Awake()
    {
        // Making sure there is exactly one instance of GUIManager
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        // Making sure the GUIManager persists accross levels
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);

        layers = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            layers.Add(transform.GetChild(i));
            for (int j = 0; j < layers[i].childCount; j++)
            {
                layers[i].GetChild(j).GetComponent<UIElement>().layer = (UILayer)i;
            }
        }
    }
    public static UIElement Instantiate(UIElement _element)
    {
        UIElement element = Instantiate(_element.gameObject).GetComponent<UIElement>();
        element.transform.SetParent(layers[(int)element.layer]);
        return element;
    }

    public static void Show(UILayer layer)
    {
        for (int i = 0; i < layers[(int)layer].childCount; i++)
        {
            UIElement element = layers[(int)layer].GetChild(i).GetComponent<UIElement>();
            element.isOpen = true;
        }
    }
    public static void Hide(UILayer layer)
    {
        for (int i = 0; i < layers[(int)layer].childCount; i++)
        {
            UIElement element = layers[(int)layer].GetChild(i).GetComponent<UIElement>();
            element.isOpen = false;
        }
    }
    public static void Toggle(UILayer layer)
    {
        for (int i = 0; i < layers[(int)layer].childCount; i++)
        {
            UIElement element = layers[(int)layer].GetChild(i).GetComponent<UIElement>();
            element.isOpen = !element.isOpen;
        }
    }


    public static void HideAll()
    {
        for (int i = 0; i < instance.transform.childCount; i++)
        {
            UIElement element = instance.transform.GetChild(i).GetComponent<UIElement>();
            element.isOpen = false;
        }
    }
    public static void ToggleAll()
    {
        for (int i = 0; i < instance.transform.childCount; i++)
        {
            UIElement element = instance.transform.GetChild(i).GetComponent<UIElement>();
            element.isOpen = !element.isOpen;
        }
    }
    public static void ShowAll()
    {
        for (int i = 0; i < instance.transform.childCount; i++)
        {
            UIElement element = instance.transform.GetChild(i).GetComponent<UIElement>();
            element.isOpen = true;
        }
    }
}
