
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActiveCardHandler : MonoBehaviour
{
    [Header("Core References")]
    [SerializeField] private Deck deck;
    [SerializeField] private Card activeCardInHand;
    [SerializeField] private Transform handTransform;
    [SerializeField] private GameObject blankCardPrefab;

    [Header("Throw Settings")]
    public float throwForce = 10f;

    [Header("Debug Settings")]
    [SerializeField] private bool useDebug = false;
    [SerializeField] private GameObject debugCardPrefab;
    [SerializeField] private ReturnLocation debugCardReturnLocation = ReturnLocation.Hand;

    private Rigidbody currentCardRb;

    private enum ReturnLocation { Hand, Deck }

    private void Awake()
    {
        if (deck == null) deck = GetComponent<Deck>();
        if (handTransform == null) Debug.LogWarning("Hand Transform not assigned.");
    }

    private void Update()
    {
        if (!useDebug) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (activeCardInHand != null && activeCardInHand.Data?.Source != null)
            {
                ThrowCardRaycast(activeCardInHand.gameObject);
            }
            else
            {
                GameObject debugCard = debugCardPrefab != null
                    ? Instantiate(debugCardPrefab)
                    : GameObject.CreatePrimitive(PrimitiveType.Cube);

                debugCard.transform.localScale = Vector3.one * 0.1f;
                debugCard.AddComponent<Rigidbody>();
                var card = debugCard.AddComponent<Card>();

                activeCardInHand = card;
                ThrowCardRaycast(debugCard);
            }
        }
    }

    private void ThrowCardRaycast(GameObject cardObject)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Rigidbody rb = cardObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                cardObject.transform.position = Camera.main.transform.position + ray.direction * 0.5f;
                rb.isKinematic = false;
                rb.linearVelocity = ray.direction * throwForce;
            }

            // Forward collision simulation to handler
            HandleCollisionWithImpact(hit.collider.gameObject, hit.point, cardObject);
        }
    }

    public void HandleCollisionWithImpact(GameObject hitObject, Vector3 impactPoint, GameObject sourceCard)
    {
        StartCoroutine(HandleCollisionWithTimeout(hitObject, impactPoint, sourceCard));
    }

    private IEnumerator HandleCollisionWithTimeout(GameObject hitObject, Vector3 impactPoint, GameObject thrownCard)
    {
        Card card = thrownCard.GetComponent<Card>();
        if (card == null)
        {
            Debug.LogWarning("No Card component on thrown object.");
            yield break;
        }

        float timer = 0f;
        bool resolved = false;

        while (timer < 5f && !resolved)
        {
            if (hitObject != null)
            {
                Collectible collectible = hitObject.GetComponent<Collectible>();

                if (collectible != null)
                {
                    card.InitializeFromCollectible(collectible);
                    collectible.gameObject.SetActive(false);
                    Debug.Log($"Card captured: {collectible.Name}");

                    if (debugCardReturnLocation == ReturnLocation.Hand)
                        ReturnCardToHand(card);
                    else
                        deck.CardComponents.Add(card);

                    resolved = true;
                }
                else if (card.Data?.Source != null)
                {
                    GameObject go = new GameObject("Reinstantiated_" + card.Data.Source.Name);
                    go.transform.position = impactPoint;
                    go.AddComponent<Collectible>();
                    Debug.Log("Reinstantiated collectible at impact.");
                    resolved = true;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (!resolved)
        {
            Debug.Log("Timeout: returning card.");
            if (debugCardReturnLocation == ReturnLocation.Hand)
                ReturnCardToHand(card);
            else
                deck.CardComponents.Add(card);
        }
    }

    private void ReturnCardToHand(Card card)
    {
        card.transform.SetParent(handTransform);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        activeCardInHand = card;
        Debug.Log("Card returned to hand.");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ActiveCardHandler))]
    public class ActiveCardHandlerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ActiveCardHandler handler = (ActiveCardHandler)target;

            GUILayout.Space(10);
            GUILayout.Label("Manual Card Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Draw Blank Card"))
            {
                handler.DrawBlankCard();
            }

            if (GUILayout.Button("Draw Card From Deck"))
            {
                handler.DrawCardFromDeck();
            }
        }
    }
#endif

    public void DrawBlankCard()
    {
        if (blankCardPrefab == null || handTransform == null)
        {
            Debug.LogError("Missing blankCardPrefab or handTransform.");
            return;
        }

        GameObject newCard = Instantiate(blankCardPrefab, handTransform.position, handTransform.rotation, handTransform);
        activeCardInHand = newCard.GetComponent<Card>();

        if (activeCardInHand == null)
        {
            Debug.LogError("Blank card prefab is missing Card component.");
            return;
        }

        currentCardRb = newCard.GetComponent<Rigidbody>();
        if (currentCardRb != null) currentCardRb.isKinematic = true;

        Debug.Log("Blank card drawn and ready in hand.");
    }

    public void DrawCardFromDeck()
    {
        if (deck.CardComponents.Count == 0)
        {
            Debug.LogWarning("No cards in deck to draw.");
            return;
        }

        activeCardInHand = deck.CardComponents[0];
        deck.CardComponents.RemoveAt(0);

        activeCardInHand.transform.SetParent(handTransform);
        activeCardInHand.transform.localPosition = Vector3.zero;
        activeCardInHand.transform.localRotation = Quaternion.identity;

        currentCardRb = activeCardInHand.GetComponent<Rigidbody>();
        if (currentCardRb != null) currentCardRb.isKinematic = true;

        Debug.Log($"Drew filled card: {activeCardInHand.Data.CardName}");
    }
}
