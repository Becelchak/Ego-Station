using EventBusSystem;
using System;
using UnityEngine;

public class LadderLogic : MonoBehaviour, ILadder
{
    [SerializeField] private Collider2D upBoxCollider;
    [SerializeField] private Collider2D downBoxCollider;
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    [SerializeField] private string interactionText = "Ползти";
    public string InteractionText => interactionText;
    private bool isPlayerOnLadder;
    private Transform player;
    private Transform pointToTeleportPlayer;
    private Collider2D ladderGateWayCollider;
    [SerializeField] private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    private void OnEnable()
    {
        ladderGateWayCollider = transform.GetChild(0).GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.name == "Player")
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
            player = other.transform;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.IsTouching(upBoxCollider))
            {
                pointToTeleportPlayer = topPoint;
            }
            else if (collision.IsTouching(downBoxCollider))
            {
                pointToTeleportPlayer = bottomPoint;
            }
            else
                pointToTeleportPlayer = null;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.EndClimbing());
            ladderGateWayCollider.gameObject.SetActive(true);

            isPlayerOnLadder = false;
            player = null;
        }
    }

    public void Interact()
    {
        if (_isBlockInteract)
            return;

        if (isPlayerOnLadder && pointToTeleportPlayer != null)
        {
            player.position = pointToTeleportPlayer.position;
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.EndClimbing());
            ladderGateWayCollider.gameObject.SetActive(true);
            isPlayerOnLadder = false;
        }
        else
        {
            isPlayerOnLadder = true;
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.StartClimbing());
            ladderGateWayCollider.gameObject.SetActive(false);
            OnInteract?.Invoke();
        }
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }

    public bool IsPlayerOnLadder() => isPlayerOnLadder;

    public Transform GetTopPoint() => topPoint;
    public Transform GetBottomPoint() => bottomPoint;
}
