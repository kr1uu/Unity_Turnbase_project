using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemNotificationUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text itemName;
    public TMP_Text amount;

    public float lifetime = 2f;

    public void Setup(int itemID, int count)
    {
        ItemEntity item =
            ItemDatabase.Instance.GetItem(itemID);

        icon.sprite =
            ItemDatabase.Instance.GetIcon(itemID);

        itemName.text = item.name;

        amount.text = "x" + count;

        Destroy(gameObject, lifetime);
    }
}