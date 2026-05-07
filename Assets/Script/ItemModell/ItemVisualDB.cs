using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Item Visual DB")]
public class ItemVisualDatabase : ScriptableObject
{
    public List<ItemVisual> databases;

    public Sprite GetIcon(int id)
    {
        foreach (var db in databases)
        {
            Sprite icon = db.GetIcon(id);

            if (icon != null)
            {
                return icon;
            }
        }

        Debug.LogWarning(
            "Cannot find icon itemID = " + id
        );

        return null;
    }
}