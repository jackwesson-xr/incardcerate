using UnityEngine;

[System.Serializable]
public class CollectibleData
{
    [SerializeField] private string collectibleName;
    [SerializeField] private Sprite collectibleIcon;
    [TextArea]
    [SerializeField] private string description;

    public CollectibleData(string name, Sprite icon, string desc)
    {
        collectibleName = name;
        collectibleIcon = icon;
        description = desc;
    }

    public string Name => collectibleName;
    public Sprite Icon => collectibleIcon;
    public string Description => description;
}
