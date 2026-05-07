using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text nameText;
    //public GameObject selectedBorder;

    private int characterID;

    public void Setup(CharacterData data)
    {
        characterID = data.id;
        nameText.text = data.name;

        // n?u có icon thì set ? ?ây
        // icon.sprite = data.icon;
    }

    public void OnClick()
    {
        Debug.Log("CLICK: " + characterID);
        //var party = PartyManager.Instance.SelectedPlayerIDs;

        //if (party.Contains(characterID))
        //{
        //    party.Remove(characterID);
        //}
        //else
        //{
        //    if (party.Count >= 3)
        //    {
        //        Debug.Log("Team full!");
        //        return;
        //    }

        //    party.Add(characterID);
        //}
        PartyManager.Instance.ToggleCharacter(characterID);
    }
}
