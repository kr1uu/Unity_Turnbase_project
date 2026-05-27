using System.Collections.Generic;
using UnityEngine;

public class StoryFlagManager : MonoBehaviour
{
    public static StoryFlagManager Instance;
    public static System.Action<string> OnFlagSet;

    private HashSet<string> flags =
        new HashSet<string>();

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

    // =========================
    // SET FLAG
    // =========================

    public void SetFlag(string flag)
    {
        if (!flags.Contains(flag))
        {
            flags.Add(flag);

            Debug.Log($"FLAG SET: {flag}");
            OnFlagSet?.Invoke(flag);

            RefreshConditionalObjects();
        }
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

    public bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    // =========================
    // REMOVE FLAG
    // =========================

    public void RemoveFlag(string flag)
    {
        if (flags.Contains(flag))
        {
            flags.Remove(flag);
        }
    }

    // =========================
    // GET ALL
    // =========================

    public List<string> GetAllFlags()
    {
        return new List<string>(flags);
    }
}