using System;

[Serializable]
public class QuestSaveData
{
    public int questID;

    public int currentAmount;

    public bool completed;

    public bool rewarded;

    public int state;
}