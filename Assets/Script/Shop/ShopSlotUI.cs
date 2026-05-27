using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;

    public TMP_Text itemNameText;

    public TMP_Text priceText;

    public TMP_Text rarityText;

    public Button buyButton;

    [Header("DATABASE")]
    public ItemVisualDatabase visualDB;

    private ItemEntity item;

    // =====================================================
    // SETUP
    // =====================================================

    public void Setup(ItemEntity data)
    {
        item = data;

        if (item == null)
        {
            Debug.LogError("ITEM NULL");
            return;
        }

        // -------------------------
        // ICON
        // -------------------------

        if (visualDB != null)
        {
            Sprite sprite =
                visualDB.GetIcon(item.id);

            if (sprite != null)
            {
                icon.sprite = sprite;
                icon.enabled = true;
            }
            else
            {
                icon.enabled = false;

                Debug.LogWarning(
                    "Missing icon ID = " +
                    item.id
                );
            }
        }

        // -------------------------
        // NAME
        // -------------------------

        itemNameText.text =
            item.name;

        // -------------------------
        // PRICE
        // -------------------------

        priceText.text =
            item.price.ToString() + " G";

        // -------------------------
        // RARITY
        // -------------------------

        rarityText.text =
            item.rarity;

        rarityText.color =
            GetRarityColor(
                item.rarity
            );

        // -------------------------
        // BUTTON
        // -------------------------

        buyButton.onClick.RemoveAllListeners();

        buyButton.onClick.AddListener(
            Buy
        );
    }

    // =====================================================
    // BUY
    // =====================================================

    void Buy()
    {
        int playerGold =
            PlayerProgression
            .Instance
            .player
            .gold;

        // -------------------------
        // NOT ENOUGH GOLD
        // -------------------------

        if (playerGold < item.price)
        {
            Debug.Log(
                "NOT ENOUGH GOLD"
            );

            return;
        }

        // -------------------------
        // BUY
        // -------------------------

        PlayerProgression
            .Instance
            .player
            .gold -= item.price;

        InventoryManager.Instance
            .AddItem(
                item.id,
                1
            );

        ShopUI.Instance
            .RefreshGold();

        Debug.Log(
            $"BUY ITEM: {item.name}"
        );
    }

    // =====================================================
    // RARITY COLOR
    // =====================================================

    Color GetRarityColor(
        string rarity
    )
    {
        switch (rarity)
        {
            case "Common":

                return Color.white;

            case "Rare":

                return Color.blue;

            case "Epic":

                return new Color(
                    0.7f,
                    0.3f,
                    1f
                );

            case "Legend":

                return new Color(
                    1f,
                    0.5f,
                    0f
                );

            default:

                return Color.gray;
        }
    }
}