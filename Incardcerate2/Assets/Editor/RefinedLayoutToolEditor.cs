
using UnityEngine;
using UnityEditor;

public class RefinedLayoutToolEditor : EditorWindow
{
    private enum LayoutShape { Line, Grid, Circle }
    private enum OriginMode { FirstSelectedObjectPivot, WorldZero, LocalZero }

    private LayoutShape shape = LayoutShape.Line;
    private OriginMode originMode = OriginMode.WorldZero;
    private float spacing = 2.0f;
    private float radius = 5.0f;

    private bool applyRandomness = false;
    private Vector3 randomAxes = new Vector3(1, 0, 0);
    private float randomnessAmount = 0.5f;

    private static LayoutShape lastShape;
    private static OriginMode lastOriginMode;
    private static float lastSpacing;
    private static float lastRadius;
    private static bool lastApplyRandomness;
    private static Vector3 lastRandomAxes;
    private static float lastRandomnessAmount;

    [MenuItem("Tools/Refined Layout Tool %#l")]
    public static void ShowWindow()
    {
        GetWindow<RefinedLayoutToolEditor>("Refined Layout Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Layout Settings", EditorStyles.boldLabel);
        shape = (LayoutShape)EditorGUILayout.EnumPopup("Shape", shape);
        originMode = (OriginMode)EditorGUILayout.EnumPopup("Origin Mode", originMode);

        spacing = EditorGUILayout.FloatField("Spacing", spacing);
        if (shape == LayoutShape.Circle)
            radius = EditorGUILayout.FloatField("Radius", radius);

        applyRandomness = EditorGUILayout.Toggle("Apply Randomness", applyRandomness);
        if (applyRandomness)
        {
            randomnessAmount = EditorGUILayout.FloatField("Randomness Amount", randomnessAmount);
            randomAxes = EditorGUILayout.Vector3Field("Random Axes (1 = On, 0 = Off)", randomAxes);
        }

        if (GUILayout.Button("Apply Layout to Selected"))
        {
            SaveLastSettings();
            ApplyLayout();
        }

        if (GUILayout.Button("Repeat Last Layout (Cmd/Ctrl + L)"))
        {
            ApplyLastLayout();
        }
    }

    [MenuItem("Tools/Repeat Refined Layout %l")]
    private static void ApplyLastLayout()
    {
        var window = GetWindow<RefinedLayoutToolEditor>();
        window.shape = lastShape;
        window.originMode = lastOriginMode;
        window.spacing = lastSpacing;
        window.radius = lastRadius;
        window.applyRandomness = lastApplyRandomness;
        window.randomAxes = lastRandomAxes;
        window.randomnessAmount = lastRandomnessAmount;

        window.ApplyLayout();
    }

    private void SaveLastSettings()
    {
        lastShape = shape;
        lastOriginMode = originMode;
        lastSpacing = spacing;
        lastRadius = radius;
        lastApplyRandomness = applyRandomness;
        lastRandomAxes = randomAxes;
        lastRandomnessAmount = randomnessAmount;
    }

    private void ApplyLayout()
    {
        var selected = Selection.transforms;
        if (selected.Length == 0)
        {
            Debug.LogWarning("No objects selected.");
            return;
        }

        Vector3 baseOrigin = Vector3.zero;

        switch (originMode)
        {
            case OriginMode.WorldZero:
                baseOrigin = Vector3.zero;
                break;

            case OriginMode.LocalZero:
                baseOrigin = selected[0].position;
                break;

            case OriginMode.FirstSelectedObjectPivot:
                baseOrigin = selected[0].position;
                break;
        }

        for (int i = 0; i < selected.Length; i++)
        {
            Vector3 layoutPos = Vector3.zero;

            switch (shape)
            {
                case LayoutShape.Line:
                    layoutPos = new Vector3(i * spacing, 0, 0);
                    break;

                case LayoutShape.Grid:
                    int gridSize = Mathf.CeilToInt(Mathf.Sqrt(selected.Length));
                    int row = i / gridSize;
                    int col = i % gridSize;
                    layoutPos = new Vector3(col * spacing, 0, row * spacing);
                    break;

                case LayoutShape.Circle:
                    float angle = i * Mathf.PI * 2 / selected.Length;
                    layoutPos = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                    break;
            }

            if (applyRandomness)
            {
                layoutPos.x += randomAxes.x != 0 ? Random.Range(-randomnessAmount, randomnessAmount) : 0;
                layoutPos.y += randomAxes.y != 0 ? Random.Range(-randomnessAmount, randomnessAmount) : 0;
                layoutPos.z += randomAxes.z != 0 ? Random.Range(-randomnessAmount, randomnessAmount) : 0;
            }

            Vector3 finalPosition = baseOrigin + layoutPos;

            Undo.RecordObject(selected[i], "Refined Layout");
            selected[i].position = finalPosition;
        }
    }
}
