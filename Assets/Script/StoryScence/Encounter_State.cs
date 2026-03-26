using System.Collections.Generic;
using UnityEngine;

public class EncounterStateManager : MonoBehaviour
{
    public static EncounterStateManager Instance;

    private Dictionary<string, HashSet<string>> defeatedByChunk
        = new Dictionary<string, HashSet<string>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void MarkDefeated(string chunkID, string encounterID)
    {
        if (!defeatedByChunk.ContainsKey(chunkID))
            defeatedByChunk[chunkID] = new HashSet<string>();

        defeatedByChunk[chunkID].Add(encounterID);
    }

    public bool IsDefeated(string chunkID, string encounterID)
    {
        return defeatedByChunk.ContainsKey(chunkID) &&
               defeatedByChunk[chunkID].Contains(encounterID);
    }

public void ResetChunk(string chunkID)
{
    defeatedByChunk.Remove(chunkID);
}
}
