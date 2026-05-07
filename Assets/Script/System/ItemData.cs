using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite icon;
    public int bonusATK;
    public int bonusDEF;
    public int bonusHP;

    public ItemType type;
    public enum ItemType
    {
        Weapon,
        Armor,
        Accessory,
        Consumable
    }

}
