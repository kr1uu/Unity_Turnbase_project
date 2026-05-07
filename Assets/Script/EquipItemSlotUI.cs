using UnityEngine;
using UnityEngine.UI;

public class EquipItemSlotUI : MonoBehaviour
{
    public Image icon;

    private int itemID;
    private CharacterPanelUI panel;

    public void Setup(
        int id,
        CharacterPanelUI ui
    )
    {
        itemID = id;
        panel = ui;

        icon.sprite =
            ItemDatabase.Instance.GetIcon(id);
    }

    public void OnClick()
    {
        panel.Equip(itemID);
    }
}