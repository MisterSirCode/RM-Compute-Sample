using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Raymarch : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    private void OnRenderImage(RenderTexture src, RenderTexture dest) {
        renderTexture = new RenderTexture(256, 256, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        computeShader.SetTexture(0, "Result", renderTexture);
        computeShader.SetFloat("res", renderTexture.width);
        computeShader.Dispatch(0, renderTexture.width / 8, renderTexture.height / 8, 1);
        Graphics.Blit(renderTexture, dest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
