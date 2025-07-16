/*using UnityEngine;
using UnityEditor;
using System.IO;

public class TradingCardIconGenerator : EditorWindow
{
    private GameObject modelPrefab;
    private Camera renderCam;
    private RenderTexture renderTexture;
    private string outputFolder = "Assets/TradingCardIcons";
    private int textureSize = 512;

    [MenuItem("Tools/Trading Card Icon Generator")]
    public static void ShowWindow()
    {
        GetWindow<TradingCardIconGenerator>("Icon Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Render Settings", EditorStyles.boldLabel);

        modelPrefab = (GameObject)EditorGUILayout.ObjectField("Model Prefab", modelPrefab, typeof(GameObject), false);
        renderCam = (Camera)EditorGUILayout.ObjectField("Render Camera", renderCam, typeof(Camera), true);
        textureSize = EditorGUILayout.IntField("Texture Size", textureSize);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Generate Icon"))
        {
            if (modelPrefab == null || renderCam == null)
            {
                Debug.LogError("Missing prefab or camera reference.");
                return;
            }

            GenerateIcon();
        }
    }

    private void GenerateIcon()
    {
        GameObject instance = Instantiate(modelPrefab);
        instance.transform.position = Vector3.zero;
        instance.transform.rotation = Quaternion.identity;
        instance.layer = LayerMask.NameToLayer("Default");

        // Get bounds
        Bounds bounds = GetBounds(instance);

        // Position camera
        renderCam.orthographic = true;
        renderCam.orthographicSize = Mathf.Max(bounds.extents.x, bounds.extents.y);
        renderCam.transform.position = bounds.center + new Vector3(0, 0, -10);
        renderCam.transform.LookAt(bounds.center);

        // Set up RenderTexture
        renderTexture = new RenderTexture(textureSize, textureSize, 24);
        renderCam.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        // Render
        renderCam.Render();

        // Read pixels
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        texture.ReadPixels(new Rect(0, 0, textureSize, textureSize), 0, 0);
        texture.Apply();

        // Save to PNG
        byte[] bytes = texture.EncodeToPNG();
        string filename = Path.Combine(outputFolder, modelPrefab.name + ".png");
        Directory.CreateDirectory(outputFolder);
        File.WriteAllBytes(filename, bytes);

        // Cleanup
        RenderTexture.active = null;
        renderCam.targetTexture = null;
        DestroyImmediate(texture);
        DestroyImmediate(renderTexture);
        DestroyImmediate(instance);

        AssetDatabase.Refresh();
        Debug.Log("Icon saved to " + filename);
    }

    private Bounds GetBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }
}
*/