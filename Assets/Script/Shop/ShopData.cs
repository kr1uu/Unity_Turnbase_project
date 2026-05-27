using SQLite4Unity3d;
[Table("Shops")]
public class ShopData
{
    [PrimaryKey]
    public int id { get; set; }

    public string shop_name { get; set; }

    public string shop_type { get; set; }

    public int shop_tier { get; set; }
}