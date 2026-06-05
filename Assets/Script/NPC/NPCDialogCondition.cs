using SQLite4Unity3d;
[Table("npc_dialogue_conditions")]
public class NPCDialogueCondition
{
    [PrimaryKey]
    public int id { get; set; }

    public int npc_id { get; set; }

    public int required_flag { get; set; }

    public int required_quest_id { get; set; }

    public string required_quest_state { get; set; }

    public int dialogue_group_id { get; set; }

    public int priority { get; set; }
}