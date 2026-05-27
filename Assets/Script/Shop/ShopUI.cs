using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("UI")]
    public GameObject panel;

    public Transform content;

    public TMP_Text goldText;

    public GameObject slotPrefab;

    private List<ItemEntity> currentItems =
        new();

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }

    // =====================================================
    // OPEN SHOP
    // =====================================================

    public void Open(ShopData shop)
    {
        if (shop == null)
        {
            Debug.LogError("SHOP NULL");
            return;
        }

        panel.SetActive(true);

        RefreshGold();

        GenerateShop(shop);

        Debug.Log(
            $"OPEN SHOP: {shop.shop_name}"
        );
    }

    // =====================================================
    // GENERATE SHOP
    // =====================================================

    void GenerateShop(ShopData shop)
    {
        // -------------------------
        // CLEAR OLD
        // -------------------------

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        currentItems.Clear();

        // -------------------------
        // FILTER BY TYPE
        // -------------------------

        List<ItemEntity> pool =
            ItemDatabase.Instance.items
            .Where(x =>
                !string.IsNullOrEmpty(x.type) &&
                !string.IsNullOrEmpty(shop.shop_type) &&
                x.type.Trim().ToLower() ==
                shop.shop_type.Trim().ToLower()
            )
            .ToList();

        // -------------------------
        // FILTER BY TIER
        // -------------------------

        List<ItemEntity> valid =
            pool
            .Where(item =>
            {
                string rarity =
                    item.rarity
                    .Trim()
                    .ToLower();

                switch (shop.shop_tier)
                {
                    case 1:

                        return rarity == "common" ||
                               (rarity == "rare" &&
                                Random.Range(1, 101) <= 15);

                    case 2:

                        return rarity == "common" ||
                               rarity == "rare";

                    case 3:

                        return rarity == "rare" ||
                               rarity == "epic";

                    default:

                        return true;
                }
            })
            .OrderBy(x => Random.value)
            .Take(6)
            .ToList();

        Debug.Log(
            $"VALID COUNT = {valid.Count}"
        );

        // -------------------------
        // CREATE UI
        // -------------------------

        foreach (var item in valid)
        {
            currentItems.Add(item);

            GameObject go =
                Instantiate(
                    slotPrefab,
                    content
                );

            ShopSlotUI ui =
                go.GetComponent<ShopSlotUI>();

            if (ui == null)
            {
                Debug.LogError(
                    "ShopSlotUI missing!"
                );

                continue;
            }

            ui.Setup(item);

            Debug.Log(
                $"SHOP ITEM: {item.name}"
            );
        }
    }
    // =====================================================
    // RANDOM ITEM BY TIER
    // =====================================================

    ItemEntity GetRandomItemByTier(
     List<ItemEntity> pool,
     int tier
 )
    {
        foreach (var item in pool)
        {
            Debug.Log(
                $"ITEM={item.name} | RARITY=[{item.rarity}] | TIER={tier}"
            );
        }
        List<ItemEntity> valid =
            new();

        foreach (var item in pool)
        {
            if (string.IsNullOrEmpty(item.rarity))
                continue;

            string rarity =
                item.rarity
                .Trim()
                .ToLower();

            int roll =
                Random.Range(1, 101);

            bool allow = false;

            switch (tier)
            {
                // =========================
                // TIER 1
                // =========================

                case 1:

                    if (rarity == "common")
                    {
                        allow = true;
                    }

                    else if (
                        rarity == "rare" &&
                        roll <= 15
                    )
                    {
                        allow = true;
                    }

                    break;

                // =========================
                // TIER 2
                // =========================

                case 2:

                    if (
                        rarity == "common" ||
                        rarity == "rare"
                    )
                    {
                        allow = true;
                    }

                    else if (
                        rarity == "epic" &&
                        roll <= 15
                    )
                    {
                        allow = true;
                    }

                    break;

                // =========================
                // TIER 3
                // =========================

                case 3:

                    if (
                        rarity == "rare" ||
                        rarity == "epic"
                    )
                    {
                        allow = true;
                    }

                    else if (
                        rarity == "legend" &&
                        roll <= 10
                    )
                    {
                        allow = true;
                    }

                    break;

                // =========================
                // TIER 4+
                // =========================

                default:

                    allow = true;

                    break;
            }

            if (allow)
            {
                valid.Add(item);
            }
        }

        Debug.Log(
            $"VALID COUNT = {valid.Count}"
        );

        if (valid.Count == 0)
        {
            Debug.LogWarning(
                "NO VALID ITEM"
            );

            return null;
        }

        return valid[
            Random.Range(0, valid.Count)
        ];
    }
    // =====================================================
    // REFRESH GOLD
    // =====================================================

    public void RefreshGold()
    {
        goldText.text =
            "Gold : " +
            PlayerProgression
            .Instance
            .player
            .gold;
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void Close()
    {
        panel.SetActive(false);
    }
}