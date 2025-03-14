using EventBusSystem;
using System;
using UnityEngine;

public class LadderLogic : MonoBehaviour, ILadder
{
    private BoxCollider2D upBoxCollider;
    private GameObject pointUpPosition;
    private GameObject pointDownPosition;
    private bool isUsing = false;
    private GameObject player;
    /// Решить проблему с этим
    private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }
    void Start()
    {
        upBoxCollider = transform.GetChild(0).GetComponent<BoxCollider2D>();
        pointUpPosition = transform.GetChild(1).gameObject;
        pointDownPosition = transform.GetChild(2).gameObject;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        print("Ladder trigger");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        player = other.transform.gameObject;

    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        print("Ladder trigger stay");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        player = other.transform.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        print("Exit ladder trigger");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
        upBoxCollider.enabled = true;
        isUsing = false;

    }
    public void Interact()
    {
        if(isBlockInteract)
            return;

        if(!isUsing)
        {
            upBoxCollider.enabled = false;
            player.gameObject.transform.position = pointUpPosition.transform.position;
            isUsing = true;
        }
        else
        {
            upBoxCollider.enabled = true;
            player.gameObject.transform.position = pointDownPosition.transform.position;
            isUsing = false;
        }

        OnInteract?.Invoke();
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }
}
