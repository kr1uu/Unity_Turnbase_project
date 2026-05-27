public class StatusEffect
{
    public string effectType;

    public int power;

    public int duration;

    public BattleUnit source;
    public int shieldValue;
    public int currentShield;

    public bool applied = false;

    public StatusEffect(
    string effectType,
    int power,
    int duration,
    BattleUnit source = null
)
    {
        this.effectType = effectType;
        this.power = power;
        this.duration = duration;
        this.source = source;
    }
    public StatusEffect Clone()
    {
        return new StatusEffect(
            effectType,
            power,
            duration,
            source
        )
        {
            applied = false
        };
    }
}
