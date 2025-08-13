using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RepeatBehindLocalSpaceRenderer : MonoBehaviour
{
    public Material material;
    [Range(1, 1023)] public int count = 5;
    public Vector3 offset = new Vector3(0, 0, -0.5f);

    void OnEnable()
    {
        if (!material) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Matrix4x4[] matrices = new Matrix4x4[count];
        Matrix4x4 baseMatrix = transform.localToWorldMatrix;

        for (int i = 0; i < count; i++)
            matrices[i] = baseMatrix;

        material.enableInstancing = true;
        material.SetVector("_Offset", offset);

        Graphics.DrawMeshInstanced(mesh, 0, material, matrices, count);
    }
}
