using UnityEngine;
using UnityEngine.InputSystem;

public class ShootScript : MonoBehaviour
{
    public bool isLoaded = false;
    public GameObject loadedCard = null;
    public BoxCollider boxCollider;
    public InputActionReference shootAction;
    public GameObject blank;
    //public Transform transform;
    private void Awake()
    {
        shootAction.action.performed += Shoot;
    }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    void Update()
    {
        if (GameObject.Find("Card Atlas").GetComponent<CardAtlas>().hand.Count > 0) {
            boxCollider.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isLoaded)
        {
            /*if (other.gameObject.tag == "Player")
            {
                Debug.Log("BeepBoop");
            }*/

            if (other.gameObject.tag == "Card")
            {
                Debug.Log("Card Has Entered Cannon");
                isLoaded = true;
                loadedCard = other.gameObject;
                loadedCard.transform.SetParent(this.gameObject.transform);
                boxCollider.enabled = false;
                //loadedCard.transform.position = Vector3.zero;
                //loadedCard.transform.rotation = Quaternion.identity;
                shootAction.action.Enable();
                loadedCard.SetActive(false);
            }
            GameObject cardAtlas = GameObject.Find("Card Atlas");

            if (other.gameObject.tag == "Blank Card" && cardAtlas.GetComponent<CardAtlas>().blankDeck.Count > 0)
            {
                Debug.Log("Blank Card Has Entered Cannon");
                isLoaded = true;
                cardAtlas.GetComponent<CardAtlas>().blankDeck.RemoveAt(0);
                loadedCard = Instantiate(blank, other.contacts[0].point, Quaternion.identity);
                cardAtlas.GetComponent<CardAtlas>().hand.Add(loadedCard.GetComponent<Card>());
                loadedCard.transform.SetParent(this.gameObject.transform);
                shootAction.action.Enable();
                boxCollider.enabled = false;
                loadedCard.SetActive(false);

            }
        }
    }

    public float shootSpeed = 20f;

    public void Shoot(InputAction.CallbackContext context)
    {
        if (loadedCard != null)
        {
            loadedCard.SetActive(true);
            GameObject.Find("Card Atlas").GetComponent<CardAtlas>().ThrowCard();
            loadedCard.GetComponent<Collider>().attachedRigidbody.useGravity = true;
            loadedCard.gameObject.GetComponent<Collider>().attachedRigidbody.constraints = RigidbodyConstraints.None;
            loadedCard.transform.SetParent(null);
            loadedCard.GetComponent<Rigidbody>().linearVelocity = transform.forward * shootSpeed;
            shootAction.action.Disable();
            loadedCard = null;
            isLoaded = false;
        }
    }
}
