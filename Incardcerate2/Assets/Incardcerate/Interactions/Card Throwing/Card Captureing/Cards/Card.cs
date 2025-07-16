using UnityEngine;

public class Card : MonoBehaviour
{
    [Header("Collectible Source")]
    public Collectible sourceCollectible;

    [Header("Card Data")]
    [SerializeField] private CardData data;

    [Header("Runtime Rendered PNG")]
    [SerializeField] private Texture2D cardTexture;

    public CardData Data => data;
    public Texture2D CardTexture
    {
        get => cardTexture;
        set => cardTexture = value;
    }

    public void InitializeFromCollectible(Collectible collectible)
    {
        if (collectible == null) return;

        sourceCollectible = collectible;
        data = new CardData(collectible.GetData());
    }

    private void Awake()
    {
        if (sourceCollectible != null)
        {
            InitializeFromCollectible(sourceCollectible);
        }
    }

    private void OnValidate()
    {
        if (sourceCollectible != null)
        {
            InitializeFromCollectible(sourceCollectible);
        }

        if (data != null)
        {
            data.UpdateCardNameFromCollectible();
            gameObject.name = data.CardName;
        }
    }
}
