using UnityEngine;
using System.Collections;

public class CardReleaseScript : MonoBehaviour
{
    public bool hasObject = false; //to check if the card can be released or not yet
    public CaptureScript captureScript;
    public Vector3 contactPoint;
    void Start()
    {
        captureScript = GetComponent<CaptureScript>();
    }

    private void Release()
    {
        captureScript.collectedObject.SetActive(true);
        captureScript.collectedObject.transform.position = contactPoint;
        captureScript.collectedObject = null;
        Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        contactPoint = contact.point;
        if (hasObject)
        {
            Release();
        }
    }
}
