using UnityEngine;

/// <summary>
/// Spawns one of two prefabs depending on whether the 0 or 9 key is pressed.
/// </summary>
public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    [Tooltip("Prefab that spawns when pressing 0")]
    public GameObject prefabA;

    [Tooltip("Prefab that spawns when pressing 9")]
    public GameObject prefabB;

    [Header("Spawn Locations")]
    [Tooltip("Location to spawn prefab A")]
    public Transform locationA;

    [Tooltip("Location to spawn prefab B")]
    public Transform locationB;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (prefabA != null && locationA != null)
            {
                Instantiate(prefabA, locationA.position, locationA.rotation);
            }
            else
            {
                Debug.LogWarning("Prefab A or Location A not assigned.");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (prefabB != null && locationB != null)
            {
                Instantiate(prefabB, locationB.position, locationB.rotation);
            }
            else
            {
                Debug.LogWarning("Prefab B or Location B not assigned.");
            }
        }
    }
}
