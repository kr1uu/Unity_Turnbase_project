using SQLite4Unity3d;
using System;

[Table("Skills")]
public class SkillData
{
    [PrimaryKey, AutoIncrement]
    [Column("id")]
    public int id { get; set; }

    [Column("type_id")]
    public int type_id { get; set; }

    [Column("name")]
    public string name { get; set; }

    [Column("power")]
    public int power { get; set; }

    [Column("cooldown")]
    public int cooldown { get; set; }

    public int currentCooldown;

    // --- TargetType ---
    public enum TargetType { Single, AOE, Self, Ally }

    [Column("target_type")]
    public string targetTypeRaw { get; set; }

    public TargetType targetType => Enum.TryParse(targetTypeRaw, true, out TargetType result) ? result : TargetType.Single;

    // --- RangeType ---
    public enum RangeType { Melee, Ranged }

    [Column("range_type")]
    public string rangeTypeRaw { get; set; }

    public RangeType rangeType => Enum.TryParse(rangeTypeRaw, true, out RangeType result) ? result : RangeType.Melee;

    // --- SkillType ---
    public enum SkillType { 
        Attack = 1,
        Heal = 2, 
        Defense = 3,
        Buff = 4 ,
        Debuff = 5,
        DoT = 6,
        Shield = 7,
        Taunt = 8,
        Stun = 9,
        Cure = 10,
        Evade = 11,

    }
    public SkillType Type => (SkillType)type_id;

    public override string ToString()
    {
        return $"[Skill id={id}, name={name}, type={Type}, targetType={targetType}, rangeType={rangeType}, power={power}, cooldown={cooldown}, curCd={currentCooldown}]";
    }
}