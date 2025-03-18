using EventBusSystem;
using System;
using UnityEngine;

public class WireMiniGameLogic : Logic, IInteractive
{
    [SerializeField] private WireMiniGame wireMiniGame;

    [SerializeField] private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }

    public void Interact()
    {
        if (_isBlockInteract && wireMiniGame != null)
        {
            wireMiniGame.gameObject.SetActive(true);
        }
        if (_isBlockInteract) return;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }
}
