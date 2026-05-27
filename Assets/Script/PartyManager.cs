using System;
using System.Collections.Generic;
using UnityEngine;
using SQLite4Unity3d;
using System.IO;

public class PartyManager : MonoBehaviour
{
    public static PartyManager Instance;

    public List<int> SelectedPlayerIDs = new List<int>();
    public List<CharacterStats> PartyStats = new List<CharacterStats>();

    public event Action OnPartyChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleCharacter(int id)
    {
        Debug.Log("ToggleCharacter: " + id);

        if (SelectedPlayerIDs.Contains(id))
        {
            SelectedPlayerIDs.Remove(id);
        }
        else
        {
            if (SelectedPlayerIDs.Count >= 3)
            {
                Debug.Log("Team full!");
                return;
            }

            SelectedPlayerIDs.Add(id);
        }

        BuildPartyFromDB(); // ?? build lai stats
        OnPartyChanged?.Invoke(); // ?? update UI
    }
    public void RefreshParty()
    {
        BuildPartyFromDB();
    }

    public void BuildPartyFromDB()
    {
        Dictionary<int, CharacterStats> oldParty = new Dictionary<int, CharacterStats>();

        foreach (var c in PartyStats)
        {
            oldParty[c.id] = c;
        }

        PartyStats.Clear();

        string dbPath = Path.Combine(Application.streamingAssetsPath, "Datagame.db");
        var db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);

        foreach (var id in SelectedPlayerIDs)
        {
            var data = db.Find<CharacterData>(id);

            if (data == null)
            {
                Debug.LogError("Character not found: " + id);
                continue;
            }

            var newStats = new CharacterStats(
                data.id,
                data.faction_id,
                data.name,
                data.hp,
                data.atk,
                data.def,
                data.spd,
                data.baseLevel,
                data.expReward,
                data.goldReward,
                data.ai_profile_id
            );

            if (oldParty.ContainsKey(id))
            {
                var old = oldParty[id];

                newStats.weaponID = old.weaponID;
                newStats.armorID = old.armorID;
                newStats.accessoryID = old.accessoryID;

                Debug.Log($"Restore equip for {newStats.name}");
            }

            PartyStats.Add(newStats);
        }

        Debug.Log("PartyStats built: " + PartyStats.Count);
    }

}
