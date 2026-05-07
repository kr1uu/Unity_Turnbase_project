using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance;

    // l?u itemID thay vì ItemData
    public int selectedItemID = -1;

    void Awake()
    {
        Instance = this;
    }

    // =====================================================
    // SELECT ITEM
    // =====================================================

    public void SelectItem(int itemID)
    {
        selectedItemID = itemID;

        ItemEntity item =
            ItemDatabase.Instance.GetItem(itemID);

        if (item != null)
        {
            Debug.Log("Item selected: " + item.name);
        }
    }

    // =====================================================
    // EQUIP TO CHARACTER
    // =====================================================

    public void EquipTo(CharacterStats character)
    {
        if (selectedItemID == -1)
        {
            Debug.Log("No item selected");
            return;
        }

        InventoryManager.Instance.EquipItem(
            selectedItemID,
            character
        );

        ItemEntity item =
            ItemDatabase.Instance.GetItem(selectedItemID);

        if (item != null)
        {
            Debug.Log(
                $"Equipped {item.name} to {character.name}"
            );
        }

        // reset
        selectedItemID = -1;

        // refresh UI
        FindFirstObjectByType<TeamSlotsUI>()?.Refresh();
    }
}