using EventBusSystem;
using System;
using UnityEngine;

public class CrushItemLogic : MonoBehaviour, IInteractive
{
    [SerializeField] private GameObject crushedObject;
    [SerializeField] private Texture backgroundPrefab;
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private CrushMiniGame crushMiniGame;
    [SerializeField] private bool needDestroyThisItem;
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

        crushMiniGame.StartMiniGame(visualPrefab, backgroundPrefab);
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
            if(needDestroyThisItem)
                Destroy(gameObject);
        }
        else
        {
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
