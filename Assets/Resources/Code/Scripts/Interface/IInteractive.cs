using UnityEngine;

public interface IInteractive
{
    bool isBlockInteract { get; set; }
    void Interact();
    void BlockInteraction();
}
