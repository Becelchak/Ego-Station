using UnityEngine;

public interface IInteractive
{
    bool isBlockInteract { get; set; }
    string InteractionText { get; }
    event System.Action OnInteract;
    void Interact();
    void BlockInteraction();
}
