using UnityEngine;

/// <summary>
/// Attach to a Card GameObject to flag it as one-time-use.
/// </summary>
public class OneTimeUse : MonoBehaviour
{
    [Tooltip("If true, card is destroyed after releasing its object.")]
    public bool IsOneUse = true;
}
