using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public ItemNotificationUI prefab;
    public Transform container;

    private Queue<ItemNotificationData> queue = new();

    private bool showing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public void SetContainer(Transform newContainer)
    {
        container = newContainer;
    }
    public void ShowItem(int itemID, int amount)
    {
        Debug.Log("Manager: " + GetInstanceID());
        Debug.Log("Container: " + container);

        queue.Enqueue(new ItemNotificationData()
        {
            itemID = itemID,
            amount = amount
        });

        if (!showing)
            StartCoroutine(ProcessQueue());
    }

    IEnumerator ProcessQueue()
    {
        showing = true;
        Debug.Log(container);
        Debug.Log(container == null);

        while (queue.Count > 0)
        {
            ItemNotificationData data = queue.Dequeue();

            ItemNotificationUI ui =
                Instantiate(prefab, container);

            ui.Setup(data.itemID, data.amount);

            yield return new WaitForSecondsRealtime(0.35f);
        }

        showing = false;
    }
}

public class ItemNotificationData
{
    public int itemID;
    public int amount;
}