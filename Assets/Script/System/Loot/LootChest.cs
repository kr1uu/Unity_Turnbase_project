using System.Collections.Generic;
using UnityEngine;

public class LootChest : MonoBehaviour,IInteractable
{
    public string chestID;

    public List<LootEntry> lootTable = new();

    public bool opened = false;

    public List<GuaranteedReward> guaranteedRewards = new();

    public SpriteRenderer spriteRenderer;

    public Sprite closedSprite;
    public Sprite openedSprite;

    // =====================================================
    // UPDATE
    // =====================================================
    private void Start()
    {
        if (EncounterStateManager.Instance == null)
            return;

        if (ChestStateManager.Instance
            .IsOpened(chestID))
        {
            opened = true;

            RefreshVisual();
        }
    }
    public void Interact()
    {
        OpenChest();
    }

    // =====================================================
    // OPEN
    // =====================================================

    public void OpenChest()
    {
        Debug.Log(
    $"TRY OPEN | chestID={chestID} | opened={opened}"
);
        if (opened)
        {
            Debug.Log("Chest already opened.");
            return;
        }

        opened = true;
        ChestStateManager.Instance.MarkOpened(chestID);
        RefreshVisual();

        foreach (var reward in guaranteedRewards)
        {
            InventoryManager.Instance.AddItem(
                reward.itemID,
                reward.amount
            );

            Debug.Log(
                $"Guaranteed Item: " +
                $"{reward.itemID} x{reward.amount}"
            );
        }

        Debug.Log($"Opening Chest: {chestID}");

        // -------------------------
        // CALCULATE TOTAL WEIGHT
        // -------------------------

        float totalChance = 0f;

        foreach (var loot in lootTable)
        {
            totalChance += loot.dropChance;
        }

        // -------------------------
        // RANDOM
        // -------------------------

        float roll = Random.Range(0f, totalChance);

        float current = 0f;

        foreach (var loot in lootTable)
        {
            current += loot.dropChance;

            if (roll <= current)
            {
                int amount = Random.Range(
                    loot.minAmount,
                    loot.maxAmount + 1
                );

                InventoryManager.Instance.AddItem(
                    loot.itemID,
                    amount
                );

                Debug.Log(
                    $"Received Item ID {loot.itemID} x{amount}"
                );

                break;
            }
        }
    }
    public void RefreshVisual()
    {
        if (spriteRenderer == null)
            return;

        spriteRenderer.sprite =
            opened ? openedSprite : closedSprite;
    }
}