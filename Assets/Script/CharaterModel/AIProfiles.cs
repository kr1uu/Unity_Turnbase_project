using SQLite4Unity3d;

[Table("AIProfiles")]
public class AIProfile
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    [Column("name")]
    public string name { get; set; }

    [Column("description")]
    public string description { get; set; }
}