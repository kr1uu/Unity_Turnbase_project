using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusIconUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text durationText;

    void Awake()
    {
        if (iconImage == null)
        {
            iconImage =
                GetComponentInChildren<Image>();
        }

        if (durationText == null)
        {
            durationText =
                GetComponentInChildren<TMP_Text>();
        }
    }

    public void Setup(Sprite icon, int duration)
    {
        iconImage.sprite = icon;
        durationText.text = duration.ToString();
    }

    public void UpdateDuration(int duration)
    {
        durationText.text = duration.ToString();
    }
}