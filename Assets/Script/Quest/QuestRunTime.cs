[System.Serializable]
public class QuestRuntime
{
    public int questID;

    public QuestData data;

    public int currentAmount;

    public bool completed;

    public bool rewarded;

    public QuestState state;
}