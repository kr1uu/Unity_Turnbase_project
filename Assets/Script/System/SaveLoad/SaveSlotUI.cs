using TMPro;
using UnityEngine;

public class SaveSlotUI : MonoBehaviour
{
    public TMP_Text slotNameText;
    public TMP_Text levelText;
    public TMP_Text goldText;
    public TMP_Text sceneText;
    public TMP_Text saveTimeText;

    private int slotID;

    // =====================================================
    // SETUP
    // =====================================================

    public void Setup(
        int id,
        GameData data
    )
    {
        slotID = id;

        slotNameText.text =
            $"Slot {id}";

        // EMPTY SLOT
        if (data == null)
        {
            levelText.text =
                "EMPTY";

            goldText.text = "";
            sceneText.text = "";
            saveTimeText.text = "";

            return;
        }

        levelText.text =
            "LV " +
            data.player.level;

        goldText.text =
            "Gold : " +
            data.player.gold;

        sceneText.text =
            data.currentScene;

        saveTimeText.text =
            System.DateTime.Now
            .ToString();
    }

    // =====================================================
    // SAVE
    // =====================================================

    public void OnSaveClick()
    {
        SaveLoadUI.Instance
            .SaveGame(slotID);
        SaveSlotPanelUI.Instance
       .RefreshSlots();
    }

    // =====================================================
    // LOAD
    // =====================================================

    public void OnLoadClick()
    {
        SaveLoadUI.Instance
            .LoadGame(slotID);
    }
}