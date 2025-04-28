using EventBusSystem;
using System;
using UnityEngine;

public class EmptyIntecativeItem : MonoBehaviour, IInteractive
{
    [SerializeField] private bool _isBlockInteractive;
    public bool isBlockInteract { get => _isBlockInteractive; set => _isBlockInteractive = value; }

    [SerializeField] public string InteractionText => "Взаимодействие";
    [SerializeField] public bool needBlockAfterUse;

    public event Action OnInteract;

    public void BlockInteraction()
    {
        _isBlockInteractive = true;
    }

    public void Interact()
    {
        OnInteract?.Invoke();
        //Nothing
        if (needBlockAfterUse)
            BlockInteraction();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.name == "Player")
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.name == "Player" && !_isBlockInteractive)
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.name == "Player")
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
        }
    }
}
