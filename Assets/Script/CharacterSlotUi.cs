using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;

    private int characterID;

    public void Setup(CharacterData data)
    {
        characterID = data.id;
        nameText.text = data.name;

        // icon.sprite = data.icon;
    }

    public void OnClick()
    {
        Debug.Log("CLICK: " + characterID);
        PartyManager.Instance.ToggleCharacter(characterID);
    }
}
