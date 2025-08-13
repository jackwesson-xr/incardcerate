using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Creates a physics chain between the player and a targeted rigidbody hit by a raycast.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class ChainConnector : MonoBehaviour
{
    public enum ActivationKey { E = KeyCode.E, Q = KeyCode.Q, F = KeyCode.F }
    
    [Header("Input")]
    public ActivationKey chainKey = ActivationKey.E;

    [Header("Chain Settings")]
    public GameObject chainLinkPrefab;
    public int chainLength = 10;
    public float maxChainDistance = 10f;
    public Vector3 characterAnchorOffset = Vector3.zero;

    [Header("Raycast Settings")]
    public float maxRayDistance = 100f;
    public LayerMask targetLayerMask = ~0; // everything by default

    private Rigidbody characterRigidbody;
    private List<GameObject> chainLinks = new List<GameObject>();

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown((KeyCode)chainKey))
        {
            TryCreateChainFromRaycast();
        }
    }

    private void TryCreateChainFromRaycast()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No main camera found.");
            return;
        }

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance, targetLayerMask))
        {
            Rigidbody hitRb = hit.collider.attachedRigidbody;
            if (hitRb != null)
            {
                ClearPreviousChain(); // Optional: prevent duplicate chains
                CreateChain(hitRb);
            }
        }
    }

    private void CreateChain(Rigidbody targetRigidbody)
    {
        float segmentLength = maxChainDistance / chainLength;
        Vector3 start = transform.position + characterAnchorOffset;
        Vector3 end = targetRigidbody.position;
        Vector3 direction = (end - start).normalized;

        Rigidbody previousRb = characterRigidbody;

        for (int i = 0; i < chainLength; i++)
        {
            Vector3 position = Vector3.Lerp(start, end, (float)i / chainLength);
            GameObject link = Instantiate(chainLinkPrefab, position, Quaternion.identity);
            Rigidbody rb = link.GetComponent<Rigidbody>();

            ConfigurableJoint joint = link.AddComponent<ConfigurableJoint>();
            joint.connectedBody = previousRb;

            joint.xMotion = ConfigurableJointMotion.Limited;
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;

            SoftJointLimit limit = new SoftJointLimit { limit = segmentLength };
            joint.linearLimit = limit;

            JointDrive drive = new JointDrive
            {
                positionSpring = 0,
                positionDamper = 0,
                maximumForce = Mathf.Infinity
            };
            joint.xDrive = joint.yDrive = joint.zDrive = drive;

            previousRb = rb;
            chainLinks.Add(link);
        }

        ConfigurableJoint finalJoint = targetRigidbody.gameObject.AddComponent<ConfigurableJoint>();
        finalJoint.connectedBody = previousRb;
        finalJoint.xMotion = ConfigurableJointMotion.Limited;
        finalJoint.yMotion = ConfigurableJointMotion.Limited;
        finalJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit finalLimit = new SoftJointLimit { limit = segmentLength };
        finalJoint.linearLimit = finalLimit;
    }

    private void ClearPreviousChain()
    {
        foreach (var link in chainLinks)
        {
            if (link != null)
                Destroy(link);
        }
        chainLinks.Clear();

        // Remove any ConfigurableJoint added to the target object last time
        foreach (var j in FindObjectsOfType<ConfigurableJoint>())
        {
            if (j.connectedBody == characterRigidbody)
                Destroy(j);
        }
    }
}
