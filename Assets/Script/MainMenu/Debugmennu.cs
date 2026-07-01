using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DebugMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject panel;

    private void Awake()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        gameObject.SetActive(false);
#endif

        if (panel != null)
            panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("F1 Pressed");

            panel.SetActive(!panel.activeSelf);
        }
    }

    //==========================
    // PLAYER
    //==========================

    public void MaxLevel()
    {
        PlayerProgression.Instance.player.level = 99;
        PlayerProgression.Instance.player.currentExp = 0;
        UnlockManager.Instance.CheckUnlocks();

        TeamSelectUI ui = FindFirstObjectByType<TeamSelectUI>();

        if (ui != null)
        {
            Debug.Log("Refresh TeamSelectUI");
            ui.LoadCharacters();
        }
        else
        {
            Debug.LogError("Không tìm th?y TeamSelectUI trong scene!");
        }

        Debug.Log("Player Level -> 99");
    }

    //==========================
    // INVENTORY
    //==========================

    public void AddAllItems()
    {
        foreach (var item in ItemDatabase.Instance.items)
        {
            InventoryManager.Instance.AddItem(item.id, 99);
        }

        Debug.Log($"Added {ItemDatabase.Instance.items.Count} items.");
    }
}