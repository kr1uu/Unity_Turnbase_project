using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Item Visual")]
public class ItemVisual : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public int itemId;

        public Sprite icon;

        // future expansion
        // public GameObject prefab;
        // public AudioClip equipSFX;
        // public RuntimeAnimatorController animator;
    }

    public List<Entry> entries;

    // =====================================================
    // GET ICON
    // =====================================================

    public Sprite GetIcon(int id)
    {
        foreach (var e in entries)
        {
            if (e.itemId == id)
            {
                if (e.icon == null)
                {
                    Debug.LogWarning(
                        "Item ID t?n t?i nh?ng icon NULL: " + id
                    );

                    return null;
                }

                Debug.Log(
                    "Found icon for itemID = " + id
                );

                return e.icon;
            }
        }

        Debug.LogWarning(
            "Không tìm th?y itemID = " + id
        );

        return null;
    }
}