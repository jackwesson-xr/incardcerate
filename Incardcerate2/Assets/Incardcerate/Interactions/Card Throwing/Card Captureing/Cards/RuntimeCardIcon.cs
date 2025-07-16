using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// Automatically renders a 3D model into a UI-based trading card icon during runtime.
/// Optionally scalable, padding-aware, and can switch between live camera feed or static PNG.
/// Also supports PNG file saving and transparent background replacement.
/// </summary>
public class RuntimeCardIcon : MonoBehaviour
{
    [Tooltip("The orthographic camera used to render the target model.")]
    public Camera renderCamera;

    [Tooltip("The 3D model to be captured into the card icon.")]
    public GameObject targetModel;

    [Tooltip("Resolution of the icon image (width x height). Maintains 5x7 aspect ratio by default.")]
    public Vector2Int textureSize = new Vector2Int(512, 716);

    [Tooltip("Scaling factor for how large the icon appears on screen.")]
    [Range(0.1f, 5f)]
    public float screenScaleFactor = 1f;

    [Tooltip("Padding applied around the object in the icon (world space units)." )]
    public float boundingPadding = 0.2f;

    [Tooltip("If true, a static PNG image is generated and shown. If false, a live camera feed is used.")]
    public bool useStaticImage = false;

    [Tooltip("Optional background color override (alpha for transparency)." )]
    public Color backgroundColor = new Color(0, 0, 0, 0);

    [Tooltip("If true, the generated PNG will also be saved to disk.")]
    public bool savePngToDisk = true;

    private RawImage rawImage;
    private RenderTexture renderTexture;
    private GameObject canvasObj;
    public Texture2D staticTexture;

    void Start()
    {
        GenerateCardIcon();
    }

    [ContextMenu("Generate Card Icon")]
    public void GenerateCardIcon()
    {
        if (renderCamera == null || targetModel == null)
        {
            Debug.LogError("Missing render camera or target model.");
            return;
        }

        // Clean up any existing textures or UI elements
        if (renderTexture != null)
        {
            renderCamera.targetTexture = null;
            renderTexture.Release();
            Destroy(renderTexture);
        }

        if (canvasObj != null)
        {
            Destroy(canvasObj);
        }

        if (staticTexture != null)
        {
            Destroy(staticTexture);
        }

        // Create a new RenderTexture and assign it to the camera
        renderTexture = new RenderTexture(textureSize.x, textureSize.y, 24);
        renderCamera.targetTexture = renderTexture;

        // Frame the camera to fit the model with padding
        Bounds bounds = GetBounds(targetModel);
        float paddedSize = Mathf.Max(bounds.extents.x, bounds.extents.y) + boundingPadding;
        renderCamera.orthographic = true;
        renderCamera.orthographicSize = paddedSize;
        renderCamera.transform.position = bounds.center + new Vector3(0, 0, -10);
        renderCamera.transform.LookAt(bounds.center);

        // Apply background color
        Color originalColor = Camera.main.backgroundColor;
        renderCamera.clearFlags = CameraClearFlags.SolidColor;
        renderCamera.backgroundColor = backgroundColor;

        // Create the UI
        CreateIconUI();

        if (useStaticImage)
        {
            // Render the image
            RenderTexture.active = renderTexture;
            renderCamera.Render();

            // Create texture and read pixels from the render
            staticTexture = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGBA32, false);
            staticTexture.ReadPixels(new Rect(0, 0, textureSize.x, textureSize.y), 0, 0);
            staticTexture.Apply();
            RenderTexture.active = null;

            // Apply the static texture to the UI
            rawImage.texture = staticTexture;

            // Optionally save the image as a PNG
            if (savePngToDisk)
            {
                string folderPath = Path.Combine(Application.dataPath, "GeneratedCardIcons");
                Directory.CreateDirectory(folderPath);
                string filename = Path.Combine(folderPath, targetModel.name + "_CardIcon.png");
                File.WriteAllBytes(filename, staticTexture.EncodeToPNG());
                Debug.Log("Card icon saved to: " + filename);
            }

            // Detach the render texture from the camera
            renderCamera.targetTexture = null;
        }
        else
        {
            // Use the live RenderTexture directly
            rawImage.texture = renderTexture;
        }

        // Restore original background if needed
        renderCamera.backgroundColor = originalColor;
    }

    /// <summary>
    /// Dynamically creates a canvas and places a RawImage UI element in the bottom-right of the screen.
    /// </summary>
    void CreateIconUI()
    {
        canvasObj = new GameObject("CardIconCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(Screen.width, Screen.height);

        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject imageObj = new GameObject("CardIcon");
        imageObj.transform.SetParent(canvasObj.transform);
        rawImage = imageObj.AddComponent<RawImage>();

        RectTransform rectTransform = rawImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);

        float baseWidth = 100f;
        float baseHeight = 140f;
        rectTransform.sizeDelta = new Vector2(baseWidth * screenScaleFactor, baseHeight * screenScaleFactor);
        rectTransform.anchoredPosition = new Vector2(-10 * screenScaleFactor, 10 * screenScaleFactor);
    }

    /// <summary>
    /// Calculates the bounds of the object including all renderers in children.
    /// </summary>
    Bounds GetBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(go.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds;
    }

    /// <summary>
    /// Cleans up textures on destroy to prevent memory leaks.
    /// </summary>
    void OnDestroy()
    {
        if (renderTexture != null)
        {
            renderCamera.targetTexture = null;
            renderTexture.Release();
        }
        if (staticTexture != null)
        {
            Destroy(staticTexture);
        }
    }
}
