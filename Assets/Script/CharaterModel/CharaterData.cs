using SQLite4Unity3d;

[Table("Characters")]
public class CharacterData
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public string name { get; set; }
    public int faction_id { get; set; }
    public int hp { get; set; }
    public int atk { get; set; }
    public int def { get; set; }
    public int spd { get; set; }
    public int level { get; set; }
    public int exp { get; set; }
    [Column("ai_profile_id")]
    public int ai_profile_id { get; set; }
}