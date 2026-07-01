using UnityEngine;

public class NotificationCanvas : MonoBehaviour
{
    public Transform popupContainer;

    private void Awake()
    {
        Debug.Log("NotificationCanvas Awake");
        NotificationManager.Instance.SetContainer(popupContainer);
    }
}