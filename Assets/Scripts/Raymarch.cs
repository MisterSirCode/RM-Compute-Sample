using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

struct ObjectData {
    public Vector2 position;
    public Vector2 scale;
    public Vector3 color;
    public int type;
    public int operation;
    public int children;
    public static int GetSize() {
        // 2 Vec2s + 1 Vec3, 3 Ints
        return sizeof(float) * 7 + sizeof(int) * 3;
    }
}

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

    private void InitScene() {
        List<Object> objects = new List<Object>(FindObjectsOfType<Object>());
        objects.Sort((a, b) => a.operation.CompareTo(b.operation));
        List<Object> ordered = new List<Object>();
        for (int i = 0; i < objects.Count; i++) {
            if (objects[i].transform.parent == null) {
                Transform parent = objects[i].transform;
                ordered.Add(objects[i]);
                objects[i].children = parent.childCount;
                for (int j = 0; j < parent.childCount; j++) {
                    if (parent.GetChild(j).GetComponent<Object>() != null) {
                        ordered.Add(parent.GetChild(j).GetComponent<Object>());
                        ordered[ordered.Count - 1].children = 0;
                    }
                }
            }
        }
        ObjectData[] data = new ObjectData[ordered.Count];
        for (int i = 0; i < ordered.Count; i++) {
            var s = ordered[i];
            Vector3 col = new Vector3(s.color.r, s.color.g, s.color.b);
            data[i] = new ObjectData() {
                position = s.Position,
                scale = s.Scale, 
                color = col,
                type = (int)s.type,
                operation = (int)s.operation,
                children = s.children
            };
        }
        ComputeBuffer sceneBuffer = new ComputeBuffer(data.Length, ObjectData.GetSize());
        sceneBuffer.SetData(data);
        computeShader.SetBuffer(0, "objects", sceneBuffer);
        computeShader.SetInt("objectCount", data.Length);
        Vector3 cpos = camera.transform.position;
        computeShader.SetFloats("cameraPos", new float[2] { cpos.x, cpos.y });
        computeShader.SetMatrix("cameraToWorld", camera.cameraToWorldMatrix);
        float scale = 1;
        scale = camera.orthographicSize;
        computeShader.SetFloat("cameraZoom", scale);
        disposables.Add(sceneBuffer);
    }

    private void RenderData(RenderTexture source, RenderTexture destination) {
        computeShader.SetTexture(0, "MainTexture", renderTexture);
        int tgx = Mathf.CeilToInt(camera.pixelWidth / 8.0f);
        int tgy = Mathf.CeilToInt(camera.pixelHeight / 8.0f);
        computeShader.Dispatch(0, tgx, tgy, 1);
        Graphics.Blit(renderTexture, destination);
        foreach (var buffer in disposables) {
            buffer.Dispose();
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination) {
        Init();
        InitTexture();
        InitScene();
        RenderData(source, destination);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
