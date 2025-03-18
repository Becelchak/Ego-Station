using EventBusSystem;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FindItemLogic : MonoBehaviour, IInteractive
{
    [SerializeField] private FindInTableMiniGame miniGame;
    [SerializeField] private CanvasGroup uiTable;
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private List<GameObject> clutterPrefabs;
    [SerializeField] private GameObject backgroundPrefab;

    private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    private void Start()
    {
        //// Назначаем UI мини-игры
        //if (miniGame != null)
        //{
        //    miniGame.SetDialogLogic(FindObjectOfType<DialogLogic>());
        //}
    }

    public void StartMiniGame()
    {
        if (miniGame == null || uiTable == null)
        {
            Debug.LogError("Мини-игра или UI не назначены!");
            return;
        }

        // Передаем параметры в мини-игру
        miniGame.SetUI(uiTable);
        miniGame.SetItems(itemPrefabs, clutterPrefabs);
        miniGame.SetBackground(backgroundPrefab);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.gameObject.name == "Player")
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
            StartMiniGame();
        }
    }

    public void Interact()
    {
        miniGame.Raise();
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }

    //void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
    //        return;
    //    EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    //}

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }
}