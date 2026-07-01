using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSwitchBtn : MonoBehaviour
{
    public TMP_Text nameText;

    public Image icon;

    private CharacterStats stats;

    private CharacterPanelUI panel;

    public void Setup(CharacterStats s, CharacterPanelUI p)
    {
        stats = s;
        panel = p;

        nameText.text = s.name;

        Button btn = GetComponent<Button>();

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        panel.ChangeCharacter(stats);
    }
}