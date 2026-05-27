using System;

[Serializable]
public class StoryEventData
{
    public string eventID;

    public string requiredFlag;

    public string setFlag;

    public bool triggerOnce = true;

    public bool triggered = false;

    public EventType type;

    public int dialogueGroupID;

    public int questID;

    public string targetObjectName;
}

public enum EventType
{
    Dialogue,
    StartQuest,
    CompleteQuest,
    SpawnObject,
    EnableObject
}