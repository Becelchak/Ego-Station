using EventBusSystem;
using System.Collections;
using UnityEngine;

public class GatewayLogic : MonoBehaviour, IGateway
{
    [SerializeField] private float timeToClose = 2f;
    private BoxCollider2D boxCollider;
    private GameObject player;
    private Animator animator;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }
    public void Interact()
    {
        StartCoroutine(OpenGateway());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        print("Gateway trigger enter");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        print("Gateway trigger exit");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }

    private IEnumerator OpenGateway()
    {
        animator.SetTrigger("Open");
        boxCollider.enabled = false;

        yield return new WaitForSeconds(timeToClose);

        boxCollider.enabled = true;
        animator.SetTrigger("Close");

        StopCoroutine(OpenGateway());
    }
}
