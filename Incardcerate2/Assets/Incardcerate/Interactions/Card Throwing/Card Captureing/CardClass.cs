using UnityEngine;

public class CardClass : MonoBehaviour
{
    [SerializeField] private bool hasObject; //A Bool for if the card has captured something and is holding an object
    public float weight; //Each card has a weight when an object is inside of it (lets say between 0.1-10.0) the weights determin stun and a few other things
    //if Weight is 0 it is an alive object as weight doesnt matter, the lowest weight I am gonna say is 0.1, 0 will be reserved for alive objects
    //private Vector3 angleOfCard; //This is for when the card is held and thrown, having the angle can help determin how its thrown
    //private Vector3 locationInSpace; //its current world position in the game
    //private Vector3 positioningOfCamera;
    public GameObject heldObject; //The Game Object that is being captured
    [SerializeField] private bool isToughingGround; //
    public float distanceFromGround;
    public bool isObject; //A Bool for checking if this class is on a card or on an object
    public bool isAlive; //Bool for if object is a static object or alive object
    [SerializeField] private CardClass objectsClass; //if the class is on a card and is capturing an object this is a temporary place to hold the card class of that object

    public CardClass(bool hasObject, float weight, GameObject heldObject, bool isToughingGround, float distanceFromGround, bool isObject, bool isAlive)
    {
        this.hasObject = hasObject;
        this.weight = weight;
        this.heldObject = heldObject;
        this.isToughingGround = isToughingGround;
        this.distanceFromGround = distanceFromGround;
        this.isObject = isObject;
        this.isAlive = isAlive;
    }

    void Start()
    {
        if (isObject)
        {
            GetDistanceFromGround();
        }
    }

    public void SetUpClass()
    {
        SetObject();
        SetObjectClass();
    }

    public void SetObject()
    {
        if (!hasObject && !isObject) //Somehow ignoring the hasObject statment and still transforms heldObject into null when the card hits the ground which I am trying to prevent
        {
            CaptureScript cs;
            cs = transform.gameObject.GetComponent<CaptureScript>();
            heldObject = cs.collectedObject;
            hasObject = true;
        }
    }

    public void SetObjectClass()
    {
        if (hasObject)
        {
            objectsClass = heldObject.GetComponent<CardClass>();
        }
    }

    public void SetWeight()
    {
        if (!isAlive)
        {
            weight = objectsClass.weight;
        }
        else
        {
            weight = 0;
        }
    }

    public void GetDistanceFromGround()
    {
        if (isToughingGround)
        {
            LayerMask layerMask = LayerMask.GetMask("Capturable");
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, layerMask))

            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                Debug.Log("Did Hit"); 
                Debug.Log(hit.distance + ": HIT DISTANCE");
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
    }

    //For when cards are stored in a list, they can be stored by this class instead of by the whole game object, what we can maybe do is when instantiating a card pulled from the deck
    //we instantiate the prefab at a bare minimum and then add in this script with the specifications created in the class to it, this way it takes up less storage possibly, if this
    //cant be done the way I am imagining it at the moment then I suppose holding the entire gameObject is a way to go about it. (gotta refer back to Elemental Cows on this one, I did something 
    //simular)
    //
}