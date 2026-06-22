using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public List<InventorySlot> items =
        new List<InventorySlot>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =====================================================
    // ADD ITEM
    // =====================================================

    public void AddItem(int itemID, int amount)
    {
        var slot =
            items.Find(s => s.itemID == itemID);

        if (slot != null)
        {
            slot.quantity += amount;
        }
        else
        {
            items.Add(
                new InventorySlot
                {
                    itemID = itemID,
                    quantity = amount
                }
            );
        }

        Debug.Log(
            $"ADD itemID={itemID} x{amount}"
        );
    }

    // =====================================================
    // REMOVE ITEM
    // =====================================================

    public void RemoveItem(
        int itemID,
        int amount
    )
    {
        var slot =
            items.Find(s => s.itemID == itemID);

        if (slot == null)
            return;

        slot.quantity -= amount;

        if (slot.quantity <= 0)
        {
            items.Remove(slot);
        }

        Debug.Log(
            $"REMOVE itemID={itemID} x{amount}"
        );
    }
    public bool OwnsItem(int itemID)
    {
        // Trong inventory
        if (HasItem(itemID))
            return true;

        // Trong party ?ang trang b?
        foreach (var character in PartyManager.Instance.PartyStats)
        {
            if (character.weaponID == itemID)
                return true;

            if (character.armorID == itemID)
                return true;

            if (character.accessoryID == itemID)
                return true;
        }

        return false;
    }
    // =====================================================
    // HAS ITEM
    // =====================================================

    public bool HasItem(int itemID)
    {
        var slot =
            items.Find(s => s.itemID == itemID);

        return slot != null &&
               slot.quantity > 0;
    }

    // =====================================================
    // EQUIP ITEM
    // =====================================================

    public void EquipItem(
        int itemID,
        CharacterStats character
    )
    {
        ItemEntity item =
            ItemDatabase.Instance.GetItem(itemID);

        if (item == null)
        {
            Debug.LogError(
                "EquipItem() item NULL"
            );

            return;
        }

        // Không có item trong inventory
        if (!HasItem(itemID))
        {
            Debug.LogWarning(
                "Item not in inventory"
            );

            return;
        }

        switch (item.type)
        {
            // =================================================
            // WEAPON
            // =================================================

            case "Weapon":

                if (character.weaponID != -1)
                {
                    AddItem(
                        character.weaponID,
                        1
                    );
                }

                RemoveItem(itemID, 1);

                character.weaponID = itemID;

                break;

            // =================================================
            // ARMOR
            // =================================================

            case "Armor":

                if (character.armorID != -1)
                {
                    AddItem(
                        character.armorID,
                        1
                    );
                }

                RemoveItem(itemID, 1);

                character.armorID = itemID;

                break;

            // =================================================
            // ACCESSORY
            // =================================================

            case "Accessory":

                if (character.accessoryID != -1)
                {
                    AddItem(
                        character.accessoryID,
                        1
                    );
                }

                RemoveItem(itemID, 1);

                character.accessoryID = itemID;

                break;
        }

        Debug.Log(
            $"EQUIPPED {item.name} to {character.name}"
        );
    }
}