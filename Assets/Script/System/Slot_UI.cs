using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text quantityText;

    public void Setup(ItemData item, int quantity)
    {
        icon.sprite = item.icon;
        quantityText.text = quantity.ToString();
    }
}
