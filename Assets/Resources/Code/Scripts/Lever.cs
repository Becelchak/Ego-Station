using EventBusSystem;
using System;
using UnityEngine;

public class Lever : MonoBehaviour, ILever
{
    [SerializeField] private bool _isBlockInteract;
    [SerializeField] private bool _isActive;
    [SerializeField] private LeverTarget targetInteractive;

    public event Action OnInteract;

    public void Awake()
    {
    }
    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    bool ILever.isActive 
    { 
        get { return _isActive; }
        set { _isActive = value; } 
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }

    public void Interact()
    {
        OnInteract?.Invoke();
        if (!_isActive) 
        {
            _isActive = true;
            targetInteractive.RequriedLever(this);
            transform.rotation = Quaternion.Euler(0,0,180);
        }
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
