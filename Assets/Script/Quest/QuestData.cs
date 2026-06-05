using SQLite4Unity3d;

[Table("Quests")]
public class QuestData
{
    [PrimaryKey]
    public int id { get; set; }

    public string quest_name { get; set; }

    public string description { get; set; }

    public string quest_type { get; set; }

    public int target_id { get; set; }

    public int required_amount { get; set; }

    public int reward_gold { get; set; }

    public int reward_exp { get; set; }

    public int reward_item_id { get; set; }

    public int reward_item_amount { get; set; }

    public string next_quest_ids { get; set; }

    public bool is_main_quest { get; set; }

    public int story_flag_on_complete { get; set; }
}