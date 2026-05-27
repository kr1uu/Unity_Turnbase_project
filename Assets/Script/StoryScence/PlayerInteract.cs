using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private IInteractable currentInteractable;
    private float interactCooldown = 0f;

    void Update()
    {
        interactCooldown -= Time.deltaTime;

        if (DialogueUI.Instance.IsOpen())
            return;

        if (interactCooldown > 0f)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }
    public void BlockInteract(float time)
    {
        interactCooldown = time;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable != null)
        {
            currentInteractable = interactable;

            Debug.Log(
                "Can interact"
            );
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        IInteractable interactable =
            other.GetComponent<IInteractable>();

        if (interactable != null &&
            currentInteractable == interactable)
        {
            currentInteractable = null;

            Debug.Log(
                "Leave interact"
            );
        }
    }
}