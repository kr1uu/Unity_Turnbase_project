using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    private CharacterStats character;

    public void SetCharacter(CharacterStats stats, CharacterSpriteDatabase db)
    {
        character = stats;

        nameText.text = stats.name;

        var splashArt = db.GetSplashArt(stats.id);

        if (splashArt != null)
        {
            icon.sprite = splashArt;
            icon.enabled = true;
        }
        else
        {
            icon.enabled = false;
        }
    }

    public void SetEmpty()
    {
        character = null;
        nameText.text = "Empty";
        icon.enabled = false;
    }

    public void OnClick()
    {
        Debug.Log("CLICK CHARACTER SLOT");

        if (character == null)
        {
            Debug.LogError("Character NULL ? ch?a SetCharacter");
            return;
        }

        if (CharacterPanelUI.Instance == null)
        {
            Debug.LogError("CharacterPanelUI ch?a có trong scene");
            return;
        }

        CharacterPanelUI.Instance.Open(character);

        //EquipManager.Instance.EquipTo(character);
    }

}
