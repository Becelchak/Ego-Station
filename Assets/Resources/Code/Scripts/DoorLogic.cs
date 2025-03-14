using EventBusSystem;
using UnityEngine;
using Player;
using System.Collections;
using System;

public class DoorLogic : MonoBehaviour, IDoor
{
    [SerializeField] private DoorLogic connetedDoor;
    private Collider2D playerCollider;
    private Animator animator;
    [SerializeField] private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

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
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        playerCollider = other;

    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player" || isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        playerCollider = other;

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }

    public void Interact()
    {
        if(connetedDoor != null && !isBlockInteract)   
        {
            StartCoroutine(OpenAndTeleport());
            OnInteract?.Invoke();
        }
    }

    private IEnumerator OpenAndTeleport()
    {
        Debug.Log("Before");
        animator.StopPlayback();
        animator.SetTrigger("Open");

        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationLength);

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

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }
}
