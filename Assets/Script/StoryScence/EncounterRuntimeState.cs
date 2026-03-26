using UnityEngine;

public class EncounterRuntimeState : MonoBehaviour
{
    public static EncounterRuntimeState Instance;

    public EncounterMarker lastEncounter;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Clear()
    {
        lastEncounter = null;
    }
}
