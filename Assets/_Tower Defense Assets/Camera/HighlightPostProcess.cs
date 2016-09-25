using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;

    [ExecuteInEditMode]
public class HighlightPostProcess : MonoBehaviour {
    RenderTexture m_highlightRenderTexture;
    RenderTargetIdentifier m_rtID;
    CommandBuffer m_renderBuffer;
    public Material m_highlightMaterial;
    RenderTexture m_renderTexture;
    BlurOptimized m_blur;

    public List<Highlightable> highlightedObjects;

    void Start()
    {
        m_blur = GetComponent<BlurOptimized>();
        CreateBuffers();
    }

    private void CreateBuffers()
    {
        m_highlightRenderTexture = new RenderTexture(Screen.width, Screen.height, 0);
        m_rtID = new RenderTargetIdentifier(m_highlightRenderTexture);

        m_renderBuffer = new CommandBuffer();
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (highlightedObjects.Count > 0)
        {
            ClearCommandBuffers();
            RenderHighlights();

            RenderTexture renderTexture1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);

            // Blurring the image
            m_blur.OnRenderImage(m_highlightRenderTexture, renderTexture1);
            
            // Excluding the original image from the blurred image, leaving out the areal alone
            m_highlightMaterial.SetTexture("_OccludeMap", m_highlightRenderTexture);
            Graphics.Blit(renderTexture1, renderTexture1, m_highlightMaterial, 0);

            // Just combining two textures together
            m_highlightMaterial.SetTexture("_OccludeMap", renderTexture1);
            Graphics.Blit(source, destination, m_highlightMaterial, 1);

            RenderTexture.ReleaseTemporary(renderTexture1);

            RenderTexture.active = m_highlightRenderTexture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
        }
        else
        {
            Graphics.Blit(source, destination);
        }
        
    }
    private void ClearCommandBuffers()
    {
        m_renderBuffer.Clear();
        RenderTexture.active = m_renderTexture;
        GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;
    }
    private void RenderHighlights()
    {
        List<Material> mat = new List<Material>();
        m_renderBuffer.SetRenderTarget(m_rtID);
        foreach(var obj in highlightedObjects)
        {
            if (obj != null)
            {
                Material tempMat = new Material(m_highlightMaterial);
                Renderer renderer = obj.GetComponent<Renderer>();
                tempMat.SetColor("_GlowColor", obj.highlightColor);
                mat.Add(tempMat);
                m_renderBuffer.DrawRenderer(renderer, tempMat, 0, 2);
            }
        }
        RenderTexture.active = m_highlightRenderTexture;
        Graphics.ExecuteCommandBuffer(m_renderBuffer);
        RenderTexture.active = null;
    }

    public void AddObject(Highlightable obj)
    {
        if(!highlightedObjects.Exists(o => o.GetInstanceID() == obj.GetInstanceID()))
        {
            //Debug.Log(Time.renderedFrameCount + " adding highlight of " + obj.name);
            highlightedObjects.Add(obj);
        }
    }
    public void RemoveObject(Highlightable obj)
    {
        int exisitingObjectIndex = highlightedObjects.FindIndex(o => o.GetInstanceID() == obj.GetInstanceID());
        if(exisitingObjectIndex != -1)
        {
            //Debug.Log(Time.renderedFrameCount + " index: " + exisitingObjectIndex + " removing highlight of " + obj.name);
            highlightedObjects.RemoveAt(exisitingObjectIndex);
            //Debug.Log(Time.renderedFrameCount + " highlightedObjects.Count = " + highlightedObjects.Count);
        }
    }
   
}
