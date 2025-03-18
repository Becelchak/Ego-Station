using EventBusSystem;
using System;
using UnityEngine;

public class CrushItemLogic : Logic, IInteractive
{
    [SerializeField] private GameObject crushedObject;
    [SerializeField] private Texture backgroundPrefab;
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private CrushMiniGame crushMiniGame;
    [SerializeField] private bool needDestroyThisItem;
    [SerializeField] private int point = 3;
    [SerializeField] private float timeToClick = 2;
    [SerializeField] private float spawnTime = 1;
    [SerializeField] private bool havePunished = false;
    [SerializeField] private int punishedVelocity = 0;
    [Header("Dialogs")]
    [SerializeField] private DialogLogic dialogAfterSuccess;
    [SerializeField] private DialogLogic dialogAfterFailed;
    private Collider2D playerCollider;

    private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    public void BlockInteraction()
    {
        _isBlockInteract = false;
    }

    public void Interact()
    {
        if (_isBlockInteract) return;

        crushMiniGame.StartMiniGame(visualPrefab, backgroundPrefab, point, timeToClick, spawnTime);
        crushMiniGame.MiniGameComplete += OnMiniGameComplete;
        OnInteract?.Invoke();
    }

    private void OnMiniGameComplete(bool success)
    {
        crushMiniGame.MiniGameComplete -= OnMiniGameComplete;

        if (success)
        {
            var intecractComponent = crushedObject.GetComponent<IInteractive>();
            intecractComponent.isBlockInteract = false;
            Debug.Log("Hack successful! Object is now active.");
            _isBlockInteract = true;

            if (dialogAfterSuccess != null)
                dialogAfterSuccess.gameObject.SetActive(true);

            if (needDestroyThisItem)
                Destroy(gameObject);
        }
        else
        {
            if (dialogAfterFailed != null)
                dialogAfterFailed.gameObject.SetActive(true);

                if (havePunished)
            {
                EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(punishedVelocity));
            }
            Debug.Log("Hack failed! Try again.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || _isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        playerCollider = collision;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != playerCollider || _isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        playerCollider = collision;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || _isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
        crushMiniGame.EndGame();
    }
}
