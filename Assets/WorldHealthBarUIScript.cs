using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldHealthBarUIScript : MonoBehaviour {

    public Slider slider;
    public GameObject currentObject;
    [SerializeField]
    private Vector3 initialScale;
    [SerializeField]
    private float objectScale;
    public Vector3 worldOffset;

    private int maxHealth;
	// Use this for initialization
	void Start () {
        initialScale = transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        if (currentObject) UpdateUIPosition();
	}
    public void SetMaxHealth(int _maxHealth)
    {
        maxHealth = _maxHealth;
    }
    private void UpdateUIPosition()
    {
        float dist = (Camera.main.transform.position - currentObject.transform.position).magnitude;
        transform.localScale = initialScale * 1f / dist * objectScale;
        foreach (var render in GetComponentsInChildren<Renderer>())
            render.enabled = currentObject.GetComponentInChildren<Renderer>().isVisible;
        transform.position = Camera.main.WorldToScreenPoint(currentObject.transform.position + worldOffset);
    }
    public void OnHealthUpdate(int newHealth)
    {
        slider.value = (float) newHealth / maxHealth;
    }

}
