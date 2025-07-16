using UnityEngine;
using System.Collections;

public class CaptureScript : MonoBehaviour
{
    public GameObject collectedObject;
    public CapturableObjects capturableObjectsScript;
    public RuntimeCardIcon runtimeCardIconScript;
    public ParticleSystem pingEffect;
    public float delayForDestroy = 0.1f;
    public CardReleaseScript cardReleaseScript;


    void Start()
    {
        if (runtimeCardIconScript == null)
        {
            runtimeCardIconScript = FindFirstObjectByType<RuntimeCardIcon>();
            if (runtimeCardIconScript == null)
            {
                Debug.LogWarning("RuntimeCardIcon not found in the scene!");
            }
        }
        cardReleaseScript = GetComponent<CardReleaseScript>();
    }



    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {

        capturableObjectsScript = collision.gameObject.GetComponentInParent<CapturableObjects>();
        if (capturableObjectsScript != null) //Could add if hasObject (from CardClass) is false (might work to stop it from turning null)
        {
            Debug.Log(capturableObjectsScript + ":    CAPTURABLE OBJECT SCRIPT");
            collectedObject = capturableObjectsScript.gameObject;
            Debug.Log("Card Contacted Capturable Object --- " + collectedObject.name);

            //int newLayer = LayerMask.NameToLayer("CaptureLayer");

            // Recursively set layer for object and all children
            //SetLayerRecursively(collectedObject, newLayer);

            Vector3 globalPositionOfContact = collision.contacts[0].point;
            Debug.Log(globalPositionOfContact + ": LOCATION OF CONTACT");
            Instantiate(pingEffect, globalPositionOfContact, Quaternion.identity);

            runtimeCardIconScript.targetModel = collectedObject;
            runtimeCardIconScript.GenerateCardIcon();


            Renderer quadRend;

            var childFirst = this.gameObject.transform.GetChild(0);
            quadRend = childFirst.GetComponent<Renderer>(); // TAKE ME OUT 
            quadRend = childFirst.GetChild(0).GetComponent<Renderer>(); // These 4 lines are only needed in new prefab setup, for the purpose of testing I am removing them atm
            quadRend.material.mainTexture = runtimeCardIconScript.staticTexture;
            quadRend = childFirst.GetChild(1).GetComponent<Renderer>();
            quadRend.material.mainTexture = runtimeCardIconScript.staticTexture; // These lines will be added back in when everything is perfectly set up again

            transform.gameObject.GetComponent<Card>().sourceCollectible = collectedObject.GetComponent<Collectible>();
            transform.gameObject.GetComponent<Card>().InitializeFromCollectible(transform.gameObject.GetComponent<Card>().sourceCollectible);

            GameObject.Find("Card Atlas").GetComponent<CardAtlas>().deck.Add(this.gameObject.GetComponent<Card>());
            this.transform.SetParent(GameObject.Find("Card Atlas").transform.GetChild(0));

            //Destroy(collectedObject, delayForDestroy);
            collectedObject.SetActive(false);

            capturableObjectsScript = null;
            //GetHighestYValueOnMesh();
            //GetLowestYValueOnMesh();
            this.gameObject.SetActive(false);
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    //public void SetCardClassUp()
    //{
    //    CardClass cc;
    //    cc = transform.gameObject.GetComponent<CardClass>();
    //    cc.SetUpClass();
    //}


    /*public static Vector3 GetHighestYValueOnMesh(){
        Vector3[] verts = collectedObject.GetComponent<MeshFilter>().sharedMesh.vertices; //gets the mesh filter of the gameobject the card collides with and gets all the vertices out into an array
        Vector3 topVertex = new Vector3(0,float.NegativeInfinity,0); //sets a temp value for topVertex
        for(int i = 0; i < verts.Length; i++){ //goes through all the vectors
            Vector3 vert = collectedObject.transform.TransformPoint(verts[i]); //gets the transform of the vertex
            if(vert.y > topVertex.y){ //checks to see if that transform is higher than the previous one
                topVertex = vert; //if so it changes top vertex
            }
        }
        Debug.Log(topVertex);
        return topVertex; //Returns Top Vertex
    }

    public static Vector3 GetLowestYValueOnMesh(){
        Vector3[] verts = collectedObject.GetComponent<MeshFilter>().sharedMesh.vertices; //gets the mesh filter of the gameobject the card collides with and gets all the vertices out into an array
        Vector3 bottomVertex = new Vector3(0,Mathf.Infinity,0); //sets a temp value for bottomVertex
        for(int i = 0; i < verts.Length; i++){ //goes through all the vectors
            Vector3 vert = collectedObject.transform.TransformPoint(verts[i]); //gets the transform of the vertex
            if(vert.y < bottomVertex.y){ //checks to see if that transform is higher than the previous one
                bottomVertex = vert; //if so it changes top vertex
            }
        }
        Debug.Log(bottomVertex);
        return bottomVertex; //Returns Bottom Vertex
    }*/

}