using System;
using UnityEngine;

[Serializable]
public class CutsceneAction
{
    public CutsceneActionType type;

    public string targetName;

    public Vector3 targetPosition;

    public float duration;

    public int dialogueGroupID;

    public string flagToSet;
}

public enum CutsceneActionType
{
    MoveObject,
    Dialogue,
    Wait,
    SetFlag,
    EnableObject,
    DisableObject
}