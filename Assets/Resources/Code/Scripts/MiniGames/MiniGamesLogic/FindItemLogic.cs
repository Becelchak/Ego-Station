using EventBusSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FindItemLogic : MonoBehaviour, IInteractive
{
    [SerializeField] private FindInTableMiniGame miniGame;
    [SerializeField] private CanvasGroup uiTable;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private List<GameObject> clutterPrefabs;
    [SerializeField] private GameObject backgroundPrefab;
    [SerializeField] private string interactionText = "Взаимодействовать";
    public string InteractionText => interactionText;
    [SerializeField] private bool _isBlockInteract;
    private bool isPlayerInZone = false;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    public void StartMiniGame()
    {
        if (miniGame == null || uiTable == null)
        {
            Debug.LogError("Мини-игра или UI не назначены!");
            return;
        }

        miniGame.SetUI(uiTable);
        miniGame.SetItems(itemPrefabs, clutterPrefabs);
        miniGame.SetBackground(backgroundPrefab);
        miniGame.SetDialogLogic(this);

        isPlayerInZone = true;
        miniGame.SetPlayerInZone(isPlayerInZone);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Player" && !_isBlockInteract)
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
            StartMiniGame();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;

        isPlayerInZone = false;
        miniGame.SetPlayerInZone(isPlayerInZone);

        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }

    public void Interact()
    {
        miniGame.Raise();
        OnInteract?.Invoke();
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }
}