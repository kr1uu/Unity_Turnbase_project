using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotsUI :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Image icon;
    public TMP_Text quantityText;

    private int itemID;

    private InventoryUI inventoryUI;

    // =====================================================
    // INIT
    // =====================================================

    public void Init(InventoryUI ui)
    {
        inventoryUI = ui;

        Debug.Log("INIT SUCCESS");
    }

    // =====================================================
    // SETUP
    // =====================================================

    public void Setup(int id, int quantity)
    {
        itemID = id;

        ItemEntity item =
            ItemDatabase.Instance.GetItem(id);

        if (item == null)
        {
            Debug.LogError(
                "Item NULL ID = " + id
            );

            return;
        }

        // ICON
        if (icon != null)
        {
            icon.sprite =
                ItemDatabase.Instance.GetIcon(id);
        }

        // QUANTITY
        if (quantityText != null)
        {
            quantityText.text =
                quantity.ToString();
        }

        Debug.Log(
            "Inventory Slot Setup OK: " +
            item.name
        );
    }

    // =====================================================
    // CLICK
    // =====================================================

    public void OnClick()
    {
        Debug.Log(
            "Selected item ID = " + itemID
        );

        EquipManager.Instance
            .SelectItem(itemID);

        if (inventoryUI == null)
        {
            Debug.LogError(
                "inventoryUI NULL"
            );

            return;
        }

        inventoryUI.OnItemSelected();
    }

    // =====================================================
    // TOOLTIP
    // =====================================================

    public void OnPointerEnter(
      PointerEventData eventData
  )
    {
        ItemEntity item =
            ItemDatabase.Instance.GetItem(itemID);

        if (item != null)
        {
            ItemTooltipUI.Instance.Show(item);
        }
    }

    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        ItemTooltipUI.Instance.Hide();
    }
}