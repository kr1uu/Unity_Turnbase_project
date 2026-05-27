using SQLite4Unity3d;

[Table("items")]
public class ItemEntity
{
    [PrimaryKey]
    public int id { get; set; }

    public string name { get; set; }

    public string type { get; set; }

    // =====================================
    // STATS
    // =====================================

    public int bonusATK { get; set; }

    public int bonusDEF { get; set; }

    public int bonusHP { get; set; }

    // =====================================
    // INFO
    // =====================================

    public string description { get; set; }

    public string rarity { get; set; }

    public int price { get; set; }

    // =====================================
    // INSTANT EFFECT
    // =====================================

    public string effect { get; set; }

    public int effectValue { get; set; }

    public float effectChance { get; set; }
    // =====================================
    // STATUS EFFECT
    // =====================================

    public int statusEffectID { get; set; }

    // =====================================
    // ON HIT EFFECT
    // =====================================

    public int onHitStatusEffectID { get; set; }

    public int onHitChance { get; set; }

    public InstantEffectType GetInstantEffect()
    {
        if (
            System.Enum.TryParse(
                effect,
                out InstantEffectType result
            )
        )
        {
            return result;
        }

        return InstantEffectType.None;
    }
}