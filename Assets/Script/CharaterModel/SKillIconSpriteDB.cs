using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "RPG/Skill Icon Database")]
public class SkillIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class Entry
    {
        public int skillId;
        public Sprite icon;  
    }

    public List<Entry> entries;

    public Sprite GetIconById(int id)
    {
        Debug.Log($"[SkillIconDB] Tìm icon cho skill id = {id}");

        foreach (var e in entries)
        {
            Debug.Log($"[SkillIconDB] Entry: skillId={e.skillId}, icon={(e.icon != null)}");

            if (e.skillId == id)
            {
                Debug.Log($"[SkillIconDB] FOUND icon cho skill ID = {id}");
                return e.icon;
            }
        }

        Debug.LogWarning($"[SkillIconDB] ? KHÔNG TÌM TH?Y icon cho skill ID = {id}");
        return null;
    }

}