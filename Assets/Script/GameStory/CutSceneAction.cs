using System;
using UnityEngine;

[Serializable]
public class CutsceneAction
{
    public CutsceneActionType type;

    public string targetObjectID;

    public Vector3 targetPosition;

    public float duration;

    public int dialogueGroupID;

    public int flagToSet;

    public int questID;

    public string encounterID;

    public int encounterDatabaseID;
}

public enum CutsceneActionType
{
    MoveObject,
    Dialogue,
    Wait,
    SetFlag,
    EnableObject,
    DisableObject,
    StartQuest,
    CompleteQuest,
    StartBattle
}