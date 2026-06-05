using SQLite4Unity3d;

[Table("StoryFlags")]
public class StoryFlagData
{
    [PrimaryKey]
    public int id { get; set; }

    public string flag_name { get; set; }

    public string description { get; set; }
}