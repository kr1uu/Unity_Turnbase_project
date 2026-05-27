using SQLite4Unity3d;

[Table("NPCs")]
public class NPCData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public string npc_name { get; set; }

    public string npc_type { get; set; }

    public int dialogue_group_id { get; set; }

    public int shop_id { get; set; }

    public int quest_id { get; set; }
    public int extra_id { get; set; }
}