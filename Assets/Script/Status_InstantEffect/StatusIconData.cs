using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/Status Icon Database")]
public class StatusIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class StatusIconEntry
    {
        public string effectType;
        public Sprite icon;
    }

    public List<StatusIconEntry> icons;

    private Dictionary<string, Sprite> cache;

    public Sprite GetIcon(string effect)
    {
        if (cache == null)
        {
            cache = new Dictionary<string, Sprite>();

            foreach (var e in icons)
            {
                cache[e.effectType] = e.icon;
            }
        }

        return cache.ContainsKey(effect)
            ? cache[effect]
            : null;
    }
}