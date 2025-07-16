using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Deck : MonoBehaviour
{
    public int totalNumOfCards;
    
    [Header("Card Components in Deck")]
    [SerializeField] private List<Card> deck = new List<Card>();

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

    public List<Card> CardComponents => deck;
    public Card ActiveCard => activeCard;
    public Texture2D ActiveCardTexture => activeCardTexture;

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            (deck[i], deck[randomIndex]) = (deck[randomIndex], deck[i]);
        }

        Debug.Log("Deck shuffled.");
    }

    public void DrawCard()
    {
        if (deck.Count == 0)
        {
            Debug.LogWarning("Deck is empty.");
            return;
        }

        activeCard = deck[0];
        activeCardTexture = activeCard.CardTexture;
        Debug.Log($"Drew card: {activeCard.Data.CardName}");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(Deck))]
    public class DeckEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Deck deck = (Deck)target;

            GUILayout.Space(10);
            GUILayout.Label("Deck Controls", EditorStyles.boldLabel);

            if (GUILayout.Button("Shuffle Deck"))
            {
                deck.ShuffleDeck();
            }

            if (GUILayout.Button("Draw Card"))
            {
                deck.DrawCard();
            }
        }
    }
#endif
}
