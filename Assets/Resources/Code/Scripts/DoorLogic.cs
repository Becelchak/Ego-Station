using EventBusSystem;
using UnityEngine;
using Player;

public class DoorLogic : MonoBehaviour, IDoor
{
    [SerializeField] private DoorLogic connetedDoor;
    [SerializeField] private MoveController.CordinateSide side;
    private Collider playerCollider;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        print("Trigger");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        playerCollider = other;

    }

    public void Interact()
    {
        print("Raise");
        playerCollider.transform.position = connetedDoor.gameObject.transform.position;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.ChangePlayerSide(connetedDoor.side));
    }
}
