using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotsUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;

    private int itemID;

    private InventoryUI inventoryUI;

    public void Init(InventoryUI ui)
    {
        inventoryUI = ui;
        Debug.Log("INIT SUCCESS");
    }

    public void Setup(int id, int quantity)
    {
        itemID = id;

        var item =
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
        else
        {
            Debug.LogError(
                "Icon Image NULL"
            );
        }

        // QUANTITY
        if (quantityText != null)
        {
            quantityText.text =
                quantity.ToString();
        }
        else
        {
            Debug.LogError(
                "QuantityText NULL"
            );
        }

        Debug.Log(
            "Inventory Slot Setup OK: " +
            item.name
        );
    }

    public void OnClick()
    {
        Debug.Log(
            "Selected item ID = " + itemID
        );

        EquipManager.Instance
            .SelectItem(itemID);
        if (inventoryUI == null)
        {
            Debug.LogError("inventoryUI NULL");
            return;
        }
        inventoryUI.OnItemSelected();
    }
}