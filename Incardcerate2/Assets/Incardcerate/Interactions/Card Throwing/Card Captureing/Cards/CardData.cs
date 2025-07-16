using UnityEngine;

[System.Serializable]
public class CardData
{
    public enum CardState
    {
        Deck,
        Drawn,
        Hand,
        Thrown
    }

    [SerializeField] private string cardName;
    [SerializeField] private CollectibleData sourceCollectible;
    [SerializeField] private CardState currentState = CardState.Deck;

    public CardData(CollectibleData collectible)
    {
        sourceCollectible = collectible;
        cardName = collectible.Name + " Card";
    }

    public string CardName => cardName;
    public CollectibleData Source => sourceCollectible;
    public CardState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }

    public void UpdateCardNameFromCollectible()
    {
        cardName = sourceCollectible.Name + " Card";
    }
}
