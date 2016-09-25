using UnityEngine;
using System.Collections;

class AnimateTiledTexture : MonoBehaviour
{
    public bool disableGameObjectOnEnd = true;
    public int columns = 2;
    public int rows = 2;
    public float framesPerSecond = 10f;

    private Vector2 tileSize;

    /// <summary>
    /// The current frame to display
    /// </summary>
    private int index = 0;
    private float elapsedTime = 0f;

    private new Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }
    void Start()
    {
        PlayAnim();
    }

    void PlayAnim()
    {
        // Set the tile size of the texture (in UV units), based on the rows and columns
        tileSize = new Vector2(1f / columns, 1f / rows);
        renderer.sharedMaterial.SetTextureScale("_MainTex", tileSize);
        renderer.sharedMaterial.SetTextureOffset("_MainTex", Vector3.up * (columns - 1f) / columns);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime >= 1f/framesPerSecond)
        {
            index+= Mathf.RoundToInt(elapsedTime * framesPerSecond);
            elapsedTime = 0f;
        }
        if (index >= rows * columns)
        {
            Destroy(gameObject);
            return;
        }
        // Split into x and y indexes
        Vector2 offset = new Vector2(index % columns * 1f / columns,  
                                     1 - (index / rows * 1f / rows));
    
        renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}