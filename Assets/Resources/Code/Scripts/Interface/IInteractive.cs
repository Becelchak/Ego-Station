using UnityEngine;

public interface IInteractive
{
    bool isBlockInteract { get; set; }
    event System.Action OnInteract;
    void Interact();
    void BlockInteraction();
}
