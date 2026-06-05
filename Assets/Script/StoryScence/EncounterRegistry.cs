using System.Collections.Generic;
using UnityEngine;

public class EncounterRegistry : MonoBehaviour
{
    public static EncounterRegistry Instance;

    private Dictionary<string, BattleTrigger>
        encounters = new();

    void Awake()
    {
        Instance = this;
    }

    public void Register(
        BattleTrigger trigger
    )
    {
        if (string.IsNullOrEmpty(
            trigger.encounterID))
            return;

        encounters[trigger.encounterID]
            = trigger;
    }

    public void Unregister(
        BattleTrigger trigger
    )
    {
        if (string.IsNullOrEmpty(
            trigger.encounterID))
            return;

        encounters.Remove(
            trigger.encounterID
        );
    }

    public BattleTrigger Get(
        string encounterID
    )
    {
        encounters.TryGetValue(
            encounterID,
            out BattleTrigger trigger
        );

        return trigger;
    }

    public List<string> GetAllIDs()
    {
        return new List<string>(
            encounters.Keys
        );
    }
}