using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private string collectibleName;
    [SerializeField] private Sprite collectibleIcon;
    [TextArea] [SerializeField] private string description;

    public GameObject SourceObject => gameObject;

    private void OnValidate()
    {
        // Update name dynamically from GameObject if unset or mismatched
        if (string.IsNullOrEmpty(collectibleName) || collectibleName != gameObject.name)
        {
            collectibleName = gameObject.name;
        }
    }

    public CollectibleData GetData()
    {
        return new CollectibleData(collectibleName, collectibleIcon, description);
    }

    public string Name => collectibleName;
    public Sprite Icon => collectibleIcon;
    public string Description => description;
}
