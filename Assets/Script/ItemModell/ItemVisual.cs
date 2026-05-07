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
                return e.icon;
            }
        }

        Debug.LogWarning(
            "Missing icon for itemID = " + id
        );

        return null;
    }
}