using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CardAtlas : MonoBehaviour
{
    [SerializeField] private int totalNumOfCards;

    [Header("Card Components in Deck")]
    public List<Card> deck = new List<Card>();

    [Header("Card Components in Blank Deck")]
    [SerializeField] private List<Card> blankDeck = new List<Card>();

    [Header("Card Components in Fan")]
    [SerializeField] private List<Card> fan = new List<Card>();

    [Header("Card Components in Hand")]
    [SerializeField] private List<Card> hand = new List<Card>();

    [Header("Card Components in World")]
    [SerializeField] private List<Card> inWorld = new List<Card>();

    [Header("Active Card")]
    [SerializeField] private Card activeCard;
    [SerializeField] private Texture2D activeCardTexture;
    public GameObject prefabBlankCard;
    [SerializeField] private int fanSize = 5;


    public List<Card> CardComponents => deck;
    public Card ActiveCard => activeCard;
    public Texture2D ActiveCardTexture => activeCardTexture;

    void Start()
    {
        BlankSetter(blankDeck);
        TotalCardCounter();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }

        Debug.Log("Deck shuffled.");
    }

    public void DrawOneCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty.");
            return;
        }

        Transform fanParent = this.transform.GetChild(2);

        fan.Add(deck[0]);
        deck[0].gameObject.transform.SetParent(fanParent);
        //activeCard = deck[0];
        //activeCardTexture = activeCard.CardTexture;
        //Debug.Log($"Drew card: {activeCard.Data.CardName}");
        deck[0].gameObject.SetActive(true);
        deck.RemoveAt(0);
    }

    public void DrawFan()
    {
        if (deck.Count < 5)
        {
            Debug.LogWarning("Deck Does Not Have Enough.");
            return;
        }

        Transform fanParent = this.transform.GetChild(2);

        for (int i = 0; i < fanSize; i++)
        {
            fan.Add(deck[0]);
            deck[0].gameObject.transform.SetParent(fanParent);
            //activeCard = deck[0];
            //activeCardTexture = activeCard.CardTexture;
            //Debug.Log($"Drew card: {activeCard.Data.CardName}");
            //deck[0].gameObject.SetActive(true);
            deck.RemoveAt(0);
            
        }
    }

    public void DrawFromFan() //This is only an example draw, the real one hast to have some changes cause you can pick any of the 5 there but this is sjust for showing the movement of data
    {
        if (fan.Count == 0)
        {
            Debug.LogWarning("Fan is empty.");
            return;
        }

        Transform handParent = this.transform.GetChild(3);

        hand.Add(fan[0]);
        fan[0].gameObject.transform.SetParent(handParent);
        activeCard = fan[0];
        activeCardTexture = activeCard.CardTexture;
        Debug.Log($"Drew card: {activeCard.Data.CardName}");
        fan.RemoveAt(0);
    }

    public void DrawBlankCard()
    {
        if (blankDeck.Count == 0)
        {
            Debug.LogWarning("Blank Deck is empty.");
            return;
        }

        Transform handParent = this.transform.GetChild(3);

        hand.Add(blankDeck[0]);
        blankDeck[0].gameObject.transform.SetParent(handParent);
        activeCard = blankDeck[0];
        activeCardTexture = activeCard.CardTexture;
        Debug.Log($"Drew card: {activeCard.Data.CardName}");
        //blankDeck[0].gameObject.SetActive(true);
        blankDeck.RemoveAt(0);
    }

    public void BlankSetter(List<Card> list)
    {
        if (list == null)
            return;

        int targetCount = 20;

        /*if (list.Count < targetCount)
        {
            int itemsToAdd = targetCount - list.Count;
            for (int i = 0; i < itemsToAdd; i++)
            {
                list.Add(default(T));
            }
        }*/
        Transform childParent = this.transform.GetChild(1);
        // Add new instances if list is too short
        while (list.Count < targetCount)
        {
            GameObject newObj = Instantiate(prefabBlankCard, childParent);
            list.Add(newObj.GetComponent<Card>());
            newObj.SetActive(false);
        }
    }

    public void ThrowCard()
    {
        if (hand.Count == 0)
        {
            Debug.LogWarning("Hand is empty.");
            return;
        }

        Transform inWorldParent = this.transform.GetChild(4);

        inWorld.Add(hand[0]);
        hand[0].gameObject.transform.SetParent(inWorldParent);
        //activeCard = blankDeck[0];
        //activeCardTexture = activeCard.CardTexture;
        //Debug.Log($"Threw card: {activeCard.Data.CardName}");
        hand.RemoveAt(0);
        inWorld[0].gameObject.SetActive(true); //need to adjust for increasing size... might need to remove from existance when the card gets placed down
        inWorld[0].gameObject.GetComponent<CardReleaseScript>().hasObject = true;
    }

    public void TotalCardCounter()
    {
        totalNumOfCards = deck.Count + blankDeck.Count + fan.Count + hand.Count + inWorld.Count;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CardAtlas))]
    public class DeckEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CardAtlas cardAtlas = (CardAtlas)target;

            GUILayout.Space(10);
            GUILayout.Label("Deck Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Shuffle Deck"))
            {
                cardAtlas.ShuffleDeck();
            }

            if (GUILayout.Button("Draw One Card"))
            {
                cardAtlas.DrawOneCard();
            }

            if (GUILayout.Button("Draw Full Fan"))
            {
                cardAtlas.DrawFan();
            }

            if (GUILayout.Button("Draw From Fan"))
            {
                cardAtlas.DrawFromFan();
            }

            if (GUILayout.Button("Draw Blank Card"))
            {
                cardAtlas.DrawBlankCard();
            }

            if (GUILayout.Button("Throw Card"))
            {
                cardAtlas.ThrowCard();
            }
        }
    }
#endif
}