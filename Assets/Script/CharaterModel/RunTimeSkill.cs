public class RuntimeSkill
{
    public SkillData data;

    public int currentCooldown;

    // ===== Forward Properties =====

    public int id => data.id;
    public string name => data.name;

    public SkillData.SkillType Type => data.Type;

    public SkillData.TargetType targetType => data.targetType;

    public SkillData.RangeType rangeType => data.rangeType;

    public int power => data.power;

    public int cooldown => data.cooldown;

    public int status_effect_id => data.status_effect_id;

    // ==============================

    public RuntimeSkill(SkillData skill)
    {
        data = skill;
        currentCooldown = 0;
    }
}