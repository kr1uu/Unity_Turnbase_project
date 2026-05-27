using SQLite4Unity3d;

[Table("Dialogues")]
public class DialogueData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int group_id { get; set; }

    public int line_order { get; set; }

    public string speaker_name { get; set; }

    public string content { get; set; }
}