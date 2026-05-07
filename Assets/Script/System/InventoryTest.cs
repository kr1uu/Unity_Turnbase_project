using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    void Start()
    {
        // Weapon
        InventoryManager.Instance.AddItem(1, 1);

        // Armor
        InventoryManager.Instance.AddItem(2, 1);

        // Accessory
        InventoryManager.Instance.AddItem(3, 1);

        InventoryManager.Instance.AddItem(4, 1);

        Debug.Log("TEST ITEMS ADDED");
    }
}