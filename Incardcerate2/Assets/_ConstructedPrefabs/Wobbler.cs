using UnityEngine;

/// <summary>
/// Applies local-space Y-axis spin and X/Z-axis wobble.
/// Useful for objects with parents (e.g., floating UI or props).
/// </summary>
public class WobbleAndSpinLocal : MonoBehaviour
{
    [Header("Spin Settings")]
    [Tooltip("Degrees per second to spin on the local Y-axis.")]
    public float rotationSpeed = 90f;

    [Header("Wobble Settings")]
    [Tooltip("Degrees of local wobble on the X-axis.")]
    public float wobbleAmountX = 15f;

    [Tooltip("Degrees of local wobble on the Z-axis.")]
    public float wobbleAmountZ = 15f;

    [Tooltip("Speed multiplier for the wobble wave.")]
    public float wobbleSpeed = 2f;

    private float wobbleTime;
    private float currentYRotation;

    void Start()
    {
        currentYRotation = transform.localEulerAngles.y;
    }

    void Update()
    {
        wobbleTime += Time.deltaTime * wobbleSpeed;

        // Update Y-axis spin
        currentYRotation += rotationSpeed * Time.deltaTime;

        // Compute wobble on X and Z using sine/cosine
        float xRotation = Mathf.Sin(wobbleTime) * wobbleAmountX;
        float zRotation = Mathf.Cos(wobbleTime) * wobbleAmountZ;

        // Apply local rotation
        transform.localRotation = Quaternion.Euler(xRotation, currentYRotation, zRotation);
    }
}
