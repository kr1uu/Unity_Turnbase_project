using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Target")]
    public RectTransform target;
    public TMP_Text buttonText;

    [Header("Scale")]
    public float normalScale = 1f;
    public float hoverScale = 1.20f;
    public float speed = 8f;

    [Header("Text Color")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.9f, 0.3f);

    private Vector3 targetScale;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();

        targetScale = Vector3.one * normalScale;
        buttonText.color = normalColor;
    }

    private void Update()
    {
        target.localScale = Vector3.Lerp(
            target.localScale,
            targetScale,
            Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = Vector3.one * hoverScale;
        buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = Vector3.one * normalScale;
        buttonText.color = normalColor;
    }
}