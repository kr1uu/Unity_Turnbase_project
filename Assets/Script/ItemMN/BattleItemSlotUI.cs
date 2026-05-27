using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleItemSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text itemNameText;
    public TMP_Text quantityText;

    private int itemID;

    public void Setup(int id, int quantity)
    {
        itemID = id;

        ItemEntity item =
            ItemDatabase.Instance.GetItem(id);

        icon.sprite =
            ItemDatabase.Instance.GetIcon(id);

        itemNameText.text = item.name;

        quantityText.text = "x" + quantity;
    }

    public void OnClick()
    {
        BattleItemManager.Instance.SelectItem(itemID);
    }
}