using UnityEngine;

public class ConditionalNPC : MonoBehaviour
{
    public int requiredFlag;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool visible =
            StoryFlagManager.Instance
            .HasFlag(requiredFlag);

        gameObject.SetActive(visible);
    }
}