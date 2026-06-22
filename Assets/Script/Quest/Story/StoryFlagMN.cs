using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoryFlagManager : MonoBehaviour
{
    public static StoryFlagManager Instance;
    public static System.Action<string> OnFlagSet;

    private HashSet<int> flags =
      new HashSet<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // SET FLAG
    // =========================

    public void SetFlag(int flagID)
    {
        if (!flags.Contains(flagID))
        {
            flags.Add(flagID);

            Debug.Log($"FLAG SET: {flagID}");

            OnFlagSet?.Invoke(flagID.ToString());

            RefreshConditionalObjects();
        }
    }
    public void LoadFlags(
        List<int> loadedFlags
    )
    {
        flags.Clear();

        foreach (var flag in loadedFlags)
        {
            flags.Add(flag);
        }

        RefreshConditionalObjects();
    }

    void RefreshConditionalObjects()
    {
        foreach (var e in
            Resources.FindObjectsOfTypeAll<ConditionalEncounter>())
        {
            e.Refresh();
        }
    }

    // =========================
    // CHECK FLAG
    // =========================

    public bool HasFlag(int flagID)
    {
        return flags.Contains(flagID);
    }
    // =========================
    // REMOVE FLAG
    // =========================

    public void RemoveFlag(int flagID)
    {
        flags.Remove(flagID);
    }

    // =========================
    // GET ALL
    // =========================

    public List<int> GetAllFlags()
    {
        return flags.ToList();
    }
}