using UnityEngine;
using TMPro;

public class ItemTooltipUI : MonoBehaviour
{
    public static ItemTooltipUI Instance;

    [Header("PANEL")]
    public GameObject panel;

    [Header("TEXT")]
    public TMP_Text nameText;
    public TMP_Text rarityText;
    public TMP_Text statText;
    public TMP_Text descriptionText;

    [Header("FOLLOW MOUSE")]
    public Vector2 offset =
       new Vector2(20f, -20f);

    RectTransform rect;


    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        Instance = this;

        rect =
            panel.GetComponent<RectTransform>();

        Hide();
    }
    void Update()
    {
        if (panel.activeSelf)
        {
            FollowMouse();
        }
    }
    void FollowMouse()
    {
        rect.position =
            (Vector2)Input.mousePosition +
            offset;
    }


    // =====================================================
    // SHOW
    // =====================================================

    public void Show(ItemEntity item)
    {
        if (item == null)
        {
            Debug.LogError("Tooltip item NULL");
            return;
        }

        panel.SetActive(true);

        // -------------------------
        // NAME
        // -------------------------

        nameText.text =
            item.name;

        // -------------------------
        // RARITY
        // -------------------------

        rarityText.text =
            item.rarity;

        // rarity color
        switch (item.rarity)
        {
            case "Common":
                nameText.color = Color.white;
                break;

            case "Rare":
                nameText.color = Color.cyan;
                break;

            case "Epic":
                nameText.color =
                    new Color(0.7f, 0.3f, 1f);
                break;

            case "Legendary":
                nameText.color = Color.yellow;
                break;

            default:
                nameText.color = Color.white;
                break;
        }

        // -------------------------
        // STATS
        // -------------------------

        statText.text = "";

        if (item.bonusATK != 0)
        {
            statText.text +=
                $"ATK +{item.bonusATK}\n";
        }

        if (item.bonusDEF != 0)
        {
            statText.text +=
                $"DEF +{item.bonusDEF}\n";
        }

        if (item.bonusHP != 0)
        {
            statText.text +=
                $"HP +{item.bonusHP}\n";
        }

        // -------------------------
        // DESCRIPTION
        // -------------------------

        descriptionText.text =
            item.description;
    }

    // =====================================================
    // HIDE
    // =====================================================

    public void Hide()
    {
        panel.SetActive(false);
    }
}