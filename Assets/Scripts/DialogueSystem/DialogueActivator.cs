using UnityEngine;

public class DialogueActivator : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueObject dialogueObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement playerMovement))
        {
            playerMovement.Interactable = this;
        }
    }
    private void OnTriggerExit(Collider other)
    {
                if (other.CompareTag("Player") && other.TryGetComponent(out PlayerMovement playerMovement))
        {
            if (playerMovement.Interactable is DialogueActivator dialogueActivator && dialogueActivator ==this)
            {
                playerMovement.Interactable = null;
            }
        }
    }

    public void Interact(PlayerMovement playerMovement)
    {
        playerMovement.DialogueUI.ShowDialogue(dialogueObject);
    }
}
