using SQLite4Unity3d;

[Table("CharacterSkills")]
public class CharacterSkillData
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int id { get; set; }

    [Column("character_id")]
    public int characterId { get; set; }

    [Column("skill_id")]
    public int skillId { get; set; }
}