using System;

[Serializable]
public class StoryEventData
{
    public string eventID;

    public int requiredFlag;

    public int setFlag;

    public bool triggerOnce = true;

    public bool triggered = false;

    public EventType type;

    public int dialogueGroupID;

    public int questID;

    public string targetObjectName;

    public CutsceneData cutscene;

    public int completedFlag;
}

public enum EventType
{
    Dialogue,
    StartQuest,
    CompleteQuest,
    SpawnObject,
    EnableObject,
    PlayCutscene
}