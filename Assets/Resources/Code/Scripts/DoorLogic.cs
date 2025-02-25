using EventBusSystem;
using UnityEngine;
using Player;
using System.Collections;

public class DoorLogic : MonoBehaviour, IDoor
{
    [SerializeField] private DoorLogic connetedDoor;
    //[SerializeField] private MoveController.CordinateSide side;
    private Collider2D playerCollider;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        playerCollider = other;

    }

    public void Interact()
    {
        StartCoroutine(OpenAndTeleport());
    }

    private IEnumerator OpenAndTeleport()
    {
        Debug.Log("Before");
        animator.StopPlayback();
        animator.SetTrigger("Open");

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);

        //animator.SetTrigger("Close");
        StartCoroutine(connetedDoor.OpenAfterTeleport());
        playerCollider.transform.position = connetedDoor.gameObject.transform.position;

        StopCoroutine(OpenAndTeleport());
    }

    private IEnumerator OpenAfterTeleport()
    {
        Debug.Log("After");
        animator.SetTrigger("Open");

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);

        animator.SetTrigger("Close");

        StopCoroutine(OpenAfterTeleport());
    }
}
