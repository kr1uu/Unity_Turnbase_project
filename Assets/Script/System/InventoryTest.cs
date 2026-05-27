using UnityEngine;

public class InventoryTest : MonoBehaviour
{
    void Start()
    {

        InventoryManager.Instance.AddItem(16, 1);
        InventoryManager.Instance.AddItem(17, 1);
        InventoryManager.Instance.AddItem(18, 1);
        InventoryManager.Instance.AddItem(19, 1);
        InventoryManager.Instance.AddItem(20, 1);
        InventoryManager.Instance.AddItem(21, 1);
        InventoryManager.Instance.AddItem(22, 1);
        InventoryManager.Instance.AddItem(23, 1);
        InventoryManager.Instance.AddItem(24, 1);
        InventoryManager.Instance.AddItem(25, 1);
        InventoryManager.Instance.AddItem(3, 1);
        InventoryManager.Instance.AddItem(4, 1);
        InventoryManager.Instance.AddItem(5, 1);
        InventoryManager.Instance.AddItem(6, 1);
        InventoryManager.Instance.AddItem(7, 1);
        InventoryManager.Instance.AddItem(8, 1);
        InventoryManager.Instance.AddItem(9, 1);
        InventoryManager.Instance.AddItem(10, 1);
        InventoryManager.Instance.AddItem(11, 1);
        InventoryManager.Instance.AddItem(12, 1);
        InventoryManager.Instance.AddItem(13, 1);
        InventoryManager.Instance.AddItem(14, 1);
        InventoryManager.Instance.AddItem(15, 1);   

        Debug.Log("TEST ITEMS ADDED");
    }
}