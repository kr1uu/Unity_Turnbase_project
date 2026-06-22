using SQLite4Unity3d;
[Table("CharacterSkills")]
public class CharacterSkillLink
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }

    public int character_id { get; set; }

    public int skill_id { get; set; }
}