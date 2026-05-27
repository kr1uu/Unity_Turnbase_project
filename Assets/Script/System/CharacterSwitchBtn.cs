using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSwitchBtn : MonoBehaviour
{
    public TMP_Text nameText;

    public Image icon;

    private CharacterStats stats;

    private CharacterPanelUI panel;

    public void Setup(
        CharacterStats s,
        CharacterPanelUI p
    )
    {
        stats = s;

        panel = p;

        nameText.text = s.name;

        GetComponent<Button>()
            .onClick
            .AddListener(OnClick);
    }

    void OnClick()
    {
        panel.Open(stats);
    }
}