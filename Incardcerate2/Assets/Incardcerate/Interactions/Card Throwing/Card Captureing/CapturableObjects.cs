using UnityEngine;

/// <summary>
/// Finds the combined bounds of any visible mesh renderers or skinned mesh renderers on this GameObject or its children.
/// </summary>
public class CapturableObjects : MonoBehaviour
{
    public Bounds bounds;

    void Start()
    {
        CalculateBounds();
    }

    /// <summary>
    /// Calculates bounds by checking all child SkinnedMeshRenderers and MeshRenderers.
    /// </summary>
    public void CalculateBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(includeInactive: false);

        if (renderers.Length == 0)
        {
            Debug.LogWarning("No renderers found in children of " + gameObject.name);
            bounds = new Bounds(transform.position, Vector3.zero);
            return;
        }

        bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
        {
            bounds.Encapsulate(r.bounds);
        }

        Debug.Log("Calculated bounds for " + gameObject.name + ": center=" + bounds.center + ", size=" + bounds.size);
    }

    // Optional: visualize bounds in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}