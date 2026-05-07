using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterPanelUI : MonoBehaviour
{
    public static CharacterPanelUI Instance;

    [Header("DATABASE")]
    public CharacterSpriteDatabase spriteDB;
    public ItemVisualDatabase itemVisualDB;

    [Header("MAIN PANEL")]
    public GameObject panel;

    [Header("TEXT")]
    public TMP_Text nameText;
    public TMP_Text atkText;
    public TMP_Text defText;
    public TMP_Text hpText;

    [Header("SPLASH")]
    public Image splashImage;

    [Header("EQUIPMENT ICONS")]
    public Image weaponIcon;
    public Image armorIcon;
    public Image accessoryIcon;

    [Header("EMPTY SLOT")]
    public Sprite emptyWeaponSprite;
    public Sprite emptyArmorSprite;
    public Sprite emptyAccessorySprite;

    [Header("ITEM LIST")]
    public Transform content;
    public GameObject itemSlotPrefab;

    private CharacterStats currentCharacter;

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        Debug.Log("CharacterPanelUI Awake");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =====================================================
    // OPEN PANEL
    // =====================================================

    public void Open(CharacterStats stats)
    {
        if (stats == null)
        {
            Debug.LogError("Open() stats NULL");
            return;
        }

        currentCharacter = stats;

        Refresh();

        UIManager.Instance.Push(panel);

        Debug.Log(
            "OPEN CHARACTER PANEL: " +
            stats.name
        );
    }

    // =====================================================
    // REFRESH UI
    // =====================================================

    void Refresh()
    {
        if (currentCharacter == null)
        {
            Debug.LogError(
                "currentCharacter NULL"
            );

            return;
        }

        // -------------------------
        // TEXT
        // -------------------------
        Debug.Log("weaponID = " + currentCharacter.weaponID);
        nameText.text =
            currentCharacter.name;

        atkText.text =
            "ATK : " +
            currentCharacter.GetAttack();

        defText.text =
            "DEF : " +
            currentCharacter.GetDefense();

        hpText.text =
            "HP : " +
            currentCharacter.currentHP +
            "/" +
            currentCharacter.GetMaxHP();

        // -------------------------
        // SPLASH
        // -------------------------

        if (spriteDB != null)
        {
            Sprite splash =
                spriteDB.GetSplashArt(
                    currentCharacter.id
                );

            if (splash != null)
            {
                splashImage.sprite = splash;
                splashImage.enabled = true;
            }
            else
            {
                splashImage.enabled = false;

                Debug.LogWarning(
                    "Missing splash art ID = " +
                    currentCharacter.id
                );
            }
        }

        // -------------------------
        // EQUIPMENT ICONS
        // -------------------------
        SetIcon(
            weaponIcon,
            currentCharacter.weaponID,
            emptyWeaponSprite
        );

        SetIcon(
            armorIcon,
            currentCharacter.armorID,
            emptyArmorSprite
        );

        SetIcon(
            accessoryIcon,
            currentCharacter.accessoryID,
            emptyAccessorySprite
        );
    }

    // =====================================================
    // SET ICON
    // =====================================================

    void SetIcon( Image img, int itemID, Sprite emptySprite)
    {
        if (img == null)
        {
            Debug.LogError("Image NULL");
            return;
        }

        if (itemID == -1)
        {
            img.enabled = true;

            img.sprite = emptySprite;

            return;
        }

        Sprite icon =
            itemVisualDB.GetIcon(itemID);

        if (icon != null)
        {
            img.sprite = icon;
            img.enabled = true;
        }
        else
        {
            img.enabled = false;

            Debug.LogWarning(
                "Missing icon itemID = " +
                itemID
            );
        }
    }

    // =====================================================
    // EQUIP BUTTONS
    // =====================================================

    public void OnWeaponClick()
    {
        ShowItems("Weapon");
    }

    public void OnArmorClick()
    {
        ShowItems("Armor");
    }

    public void OnAccessoryClick()
    {
        ShowItems("Accessory");
    }

    // =====================================================
    // SHOW ITEMS
    // =====================================================

    void ShowItems(string type)
    {
        // Clear old
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Spawn new
        foreach (var slot in InventoryManager.Instance.items)
        {
            ItemEntity item =
                ItemDatabase.Instance.GetItem(
                    slot.itemID
                );

            if (item == null)
            {
                Debug.LogError(
                    "Item NULL ID = " +
                    slot.itemID
                );

                continue;
            }

            // Filter
            if ( item.type.ToLower() != type.ToLower() )
                continue;
            // Spawn slot
            GameObject go =
                Instantiate(
                    itemSlotPrefab,
                    content
                );

            EquipItemSlotUI ui =
                go.GetComponent<EquipItemSlotUI>();

            if (ui == null)
            {
                Debug.LogError(
                    "EquipItemSlotUI missing!"
                );

                continue;
            }

            ui.Setup(
                slot.itemID,
                this
            );
        }
    }

    // =====================================================
    // EQUIP
    // =====================================================

    public void Equip(int itemID)
    {
        InventoryManager.Instance.EquipItem(
            itemID,
            currentCharacter
        );

        Refresh();

        Debug.Log(
            "Equipped itemID = " +
            itemID
        );
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void Close()
    {
        UIManager.Instance.Pop();
    }
}