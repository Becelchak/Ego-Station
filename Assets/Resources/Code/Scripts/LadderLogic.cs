using EventBusSystem;
using UnityEngine;

public class LadderLogic : MonoBehaviour, ILadder
{
    private BoxCollider upBoxCollider;
    private GameObject pointUpPosition;
    private GameObject pointDownPosition;
    private bool isUsing = false;
    private GameObject player;

    void Start()
    {
        upBoxCollider = transform.GetChild(0).GetComponent<BoxCollider>();
        pointUpPosition = transform.GetChild(1).gameObject;
        pointDownPosition = transform.GetChild(2).gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        print("Ladder trigger");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        player = other.transform.gameObject;

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
            return;
        print("Exit ladder trigger");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
        upBoxCollider.enabled = true;
        isUsing = false;

    }
    public void Interact()
    {
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
    }
}
