using UnityEngine;

public class SaveSlotPanelUI : MonoBehaviour
{
    public static SaveSlotPanelUI Instance;

    public SavePanelMode currentMode = SavePanelMode.Normal;

    [Header("UI")]
    public GameObject panel;

    public Transform content;

    public GameObject slotPrefab;

    [Header("SETTINGS")]
    public int maxSlots = 3;

    // =====================================================
    // AWAKE
    // =====================================================

    void Awake()
    {
        Instance = this;

        panel.SetActive(false);
    }
    public enum SavePanelMode
    {
        Normal,
        MainMenu
    }
    // =====================================================
    // OPEN
    // =====================================================

    public void Open()
    {
        currentMode = SavePanelMode.Normal;

        panel.SetActive(true);

        RefreshSlots();
    }
    public void OpenFromMainMenu()
    {
        currentMode =
            SavePanelMode.MainMenu;

        panel.SetActive(true);

        RefreshSlots();
    }

    // =====================================================
    // CLOSE
    // =====================================================

    public void Close()
    {
        panel.SetActive(false);
    }
    public void OpenSavePanel()
    {
        SaveSlotPanelUI.Instance
            .Open();
    }
    // =====================================================
    // REFRESH
    // =====================================================

    public void RefreshSlots()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 1; i <= maxSlots; i++)
        {
            GameData data =
                SaveSystem.Load(i);

            GameObject go =
                Instantiate(
                    slotPrefab,
                    content
                );

            SaveSlotUI ui =
                go.GetComponent<SaveSlotUI>();

            ui.Setup(i, data, currentMode);
        }
    }
}
