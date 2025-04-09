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
    [SerializeField] private float xOffset = 0.1f; // Небольшое смещение для точного позиционирования

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
            {
                pointToTeleportPlayer = null;
            }
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
            // Телепортация с выравниванием по X
            Vector3 newPosition = pointToTeleportPlayer.position;
            player.position = newPosition;
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.EndClimbing());
            ladderGateWayCollider.gameObject.SetActive(true);
            isPlayerOnLadder = false;
        }
        else
        {
            // Выравнивание игрока по X при начале использования лестницы
            if (player != null)
            {
                Vector3 ladderCenter = transform.position;
                Vector3 newPlayerPosition = new Vector3(
                    ladderCenter.x + xOffset, // Центрируем по X с небольшим смещением
                    player.position.y,
                    player.position.z
                );
                player.position = newPlayerPosition;
            }

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