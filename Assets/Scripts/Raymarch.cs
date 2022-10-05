using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Raymarch : MonoBehaviour
{
    Camera camera;
    [SerializeField]
    ComputeShader computeShader;
    RenderTexture renderTexture;
    List<ComputeBuffer> disposables;

    private void Init() {
        camera = Camera.current;
        disposables = new List<ComputeBuffer>();
    }

    private void InitTexture() {
        if (renderTexture == null || renderTexture.width != camera.pixelWidth || renderTexture.height != camera.pixelHeight) {
            if (renderTexture != null) {
                renderTexture.Release();
            }
            renderTexture = new RenderTexture(camera.pixelWidth, camera.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Init();
        InitTexture();
        computeShader.SetTexture(0, "MainTexture", renderTexture);
        int tgx = Mathf.CeilToInt(camera.pixelWidth / 8.0f);
        int tgy = Mathf.CeilToInt(camera.pixelHeight / 8.0f);
        computeShader.Dispatch(0, tgx, tgy, 1);
        Graphics.Blit(renderTexture, destination);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
