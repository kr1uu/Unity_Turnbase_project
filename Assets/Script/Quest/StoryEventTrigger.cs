using UnityEngine;

public class StoryEventTrigger : MonoBehaviour
{
    public StoryEventData data;

    private bool localTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        TriggerEvent();
    }

    public void TriggerEvent()
    {
        if (localTriggered)
            return;

        // required flag
        if (!string.IsNullOrEmpty(data.requiredFlag))
        {
            if (!StoryFlagManager.Instance
                .HasFlag(data.requiredFlag))
            {
                return;
            }
        }

        switch (data.type)
        {
            case EventType.StartQuest:

                QuestManager.Instance
                    .StartQuest(data.questID);

                break;

            case EventType.CompleteQuest:

                QuestManager.Instance
                    .CompleteQuest(data.questID);

                break;

            case EventType.EnableObject:

                GameObject obj =
                    GameObject.Find(
                        data.targetObjectName
                    );

                if (obj != null)
                {
                    obj.SetActive(true);
                }

                break;
        }

        // set flag
        if (!string.IsNullOrEmpty(data.setFlag))
        {
            StoryFlagManager.Instance
                .SetFlag(data.setFlag);
        }

        if (data.triggerOnce)
        {
            localTriggered = true;
        }

        Debug.Log(
            $"EVENT TRIGGERED: {data.eventID}"
        );
    }
}