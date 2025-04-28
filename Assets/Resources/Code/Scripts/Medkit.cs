using EventBusSystem;
using System;
using UnityEngine;

public class Medkit : MonoBehaviour, IInteractive
{
    [SerializeField] private bool _isBlockInteract;
    [SerializeField] private int healthUp;
    public bool isBlockInteract { get => _isBlockInteract; set => _isBlockInteract = value; }

    public string InteractionText => "Воспользоваться";

    public event Action OnInteract;

    public void BlockInteraction()
    {
        isBlockInteract = true;
    }

    public void Interact()
    {
        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetHealth(healthUp));
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
        Destroy(gameObject);
        OnInteract?.Invoke();
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
        if (collision.CompareTag("Player") && collision.name == "Player")
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
