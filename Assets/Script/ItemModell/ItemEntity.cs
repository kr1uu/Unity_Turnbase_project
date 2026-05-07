using SQLite4Unity3d;

[Table("items")]
public class ItemEntity
{
    [PrimaryKey]
    public int id { get; set; }

    public string name { get; set; }

    public string type { get; set; }

    public int bonusATK { get; set; }

    public int bonusDEF { get; set; }

    public int bonusHP { get; set; }
}