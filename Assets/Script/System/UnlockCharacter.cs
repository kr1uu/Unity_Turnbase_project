using SQLite4Unity3d;

[Table("CharacterUnlocks")]
public class CharacterUnlockData
{
    [PrimaryKey]
    public int character_id { get; set; }

    public int unlock_level { get; set; }
}