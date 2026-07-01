using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject panel;

    public Transform content;
    public GameObject slotPrefab;

    public GameObject teamPanel;

    public void Toggle()
    {
        panel.SetActive(!panel.activeSelf);

        if (panel.activeSelf)
        {
            Refresh();
        }
    }

    public void OnItemSelected()
    {
        UIManager.Instance.Push(teamPanel);
    }

    void Refresh()
    {
        // Xoa slot cu
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Spawn slot moi
        foreach (var slot in InventoryManager.Instance.items)
        {
            // Lay item tu DB
            var itemData = ItemDatabase.Instance.GetItem(slot.itemID);

            if (itemData == null)
            {
                Debug.LogError("Item not found ID = " + slot.itemID);
                continue;
            }

            Debug.Log("Spawn item: " + itemData.name);

            GameObject go = Instantiate(slotPrefab, content);

            var ui = go.GetComponent<InventorySlotsUI>();

            if (ui == null)
            {
                Debug.LogError("InventorySlotsUI missing!");
                continue;
            }

            ui.Setup(
                slot.itemID,
                slot.quantity
            );

            ui.Init(this);
            Debug.Log("INIT CALLED");
        }
    }
}