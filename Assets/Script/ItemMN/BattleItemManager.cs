using UnityEngine;

public class BattleItemManager : MonoBehaviour
{
    public static BattleItemManager Instance;

    [Header("UI")]
    public GameObject panel;

    public Transform contentParent;

    public GameObject itemSlotPrefab;

    private int selectedItemID;

    // =====================================================
    // AWAKE
    // =====================================================

    private void Awake()
    {
        Instance = this;
    }

    // =====================================================
    // OPEN MENU
    // =====================================================

    public void OpenItemMenu()
    {
        // Only player can use this kind of power 
        if (BattleManager.Instance.CurrentUnit == null)
            return;

        if (!BattleManager.Instance.CurrentUnit.isPlayer)
        {
            Debug.Log("Not player turn");
            return;
        }
        bool active = !panel.activeSelf;

        panel.SetActive(active);

        if (!active)
            return;

        BattleManager.Instance.ui.HideArtsList();

        Refresh();
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void Close()
    {
        panel.SetActive(false);
    }

    // =====================================================
    // REFRESH
    // =====================================================

    public void Refresh()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var invItem in InventoryManager.Instance.items)
        {
            ItemEntity item =
                ItemDatabase.Instance.GetItem(
                    invItem.itemID
                );

            if (item == null)
                continue;

            if (item.type != "Consumable")
                continue;

            GameObject obj =
                Instantiate(
                    itemSlotPrefab,
                    contentParent
                );

            BattleItemSlotUI slot =
                obj.GetComponent<BattleItemSlotUI>();

            if (slot == null)
            {
                Debug.LogError(
                    "BattleItemSlotUI missing!"
                );

                continue;
            }

            slot.Setup(
                invItem.itemID,
                invItem.quantity
            );
        }
    }

    // =====================================================
    // SELECT ITEM
    // =====================================================

    public void SelectItem(int itemID)
    {
        selectedItemID = itemID;

        Debug.Log(
            "Selected consumable: " +
            itemID
        );

        BattleUnit target =
            BattleManager.Instance.SelectedTarget;

        if (target == null)
        {
            Debug.LogError(
                "No target selected!"
            );

            return;
        }

        if (!target.isPlayer)
        {
            Debug.Log(
                "Cannot use consumable on enemy"
            );

            return;
        }

        UseItem(target);
    }

    // =====================================================
    // USE ITEM
    // =====================================================

    public void UseItem(BattleUnit target)
    {
        ItemEntity item =
            ItemDatabase.Instance.GetItem(
                selectedItemID
            );

        if (item == null)
            return;

        Debug.Log(
            $"Using {item.name} on {target.stats.name}"
        );

        // =========================================
        // INSTANT EFFECT
        // =========================================

        if (item.effect != "None")
        {
            InstantEffect instant =
                InstantEffectFactory.Create(
                    item.effect,
                    item.effectValue
                );

            if (instant != null)
            {
                instant.Apply(target);
            }
        }

        // =========================================
        // STATUS EFFECT
        // =========================================

        if (item.statusEffectID > 0)
        {
            StatusEffect status =
                StatusEffectFactory.Create(
                    item.statusEffectID,
                    BattleManager.Instance.CurrentUnit
                );

            if (status != null)
            {
                target.AddEffect(status);

                Debug.Log(
                    $"{target.stats.name} gain status " +
                    status.effectType
                );
            }
        }

        // =========================================
        // REMOVE ITEM
        // =========================================

        InventoryManager.Instance.RemoveItem(
            selectedItemID,
            1
        );

        // =========================================
        // REFRESH UI
        // =========================================

        Refresh();

        Close();

        BattleManager.Instance.NextTurn();
    }
    // =====================================================
    // BUTTON
    // =====================================================

    public void OnClick()
    {
        OpenItemMenu();
    }
}