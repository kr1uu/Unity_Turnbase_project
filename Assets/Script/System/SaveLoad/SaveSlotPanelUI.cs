using UnityEngine;

public class SaveSlotPanelUI : MonoBehaviour
{
    public static SaveSlotPanelUI Instance;

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

    // =====================================================
    // OPEN
    // =====================================================

    public void Open()
    {
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

            ui.Setup(i, data);
        }
    }
}
