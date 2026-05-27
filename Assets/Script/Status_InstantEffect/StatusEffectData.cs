using SQLite4Unity3d;
[Table("status_effects")]
public class StatusEffectData
{
    [PrimaryKey]
    public int id { get; set; }

    public string name { get; set; }

    public string description { get; set; }

    public string effect_type { get; set; }

    public int power { get; set; }

    public int duration { get; set; }

    public int stackable { get; set; }

    public string tick_timing{ get; set; }
}