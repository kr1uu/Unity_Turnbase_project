using UnityEngine;

public class ConditionalEncounter : MonoBehaviour
{
    public int requiredFlag;

    void Start()
    {
        Refresh();
    }

    void OnEnable()
    {
        StoryFlagManager.OnFlagSet += OnFlagChanged;
    }

    void OnDisable()
    {
        StoryFlagManager.OnFlagSet -= OnFlagChanged;
    }

    void OnFlagChanged(string flag)
    {
        Refresh();
    }

    public void Refresh()
    {
        bool unlocked =
            StoryFlagManager.Instance
            .HasFlag(requiredFlag);

        gameObject.SetActive(unlocked);
    }
}