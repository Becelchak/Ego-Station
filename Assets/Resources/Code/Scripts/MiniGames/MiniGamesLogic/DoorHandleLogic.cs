using EventBusSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

public class DoorHandleLogic : MonoBehaviour, IInteractive
{
    [Header("Configuration")]
    public DoorHandleMiniGame miniGameConfig;
    [SerializeField] private GameObject canvasPrefab;
    private GameObject newCanvas;
    private CanvasGroup uiContainer;
    private Image doorImage;
    private RectTransform handleArea;
    [SerializeField] private string interactionText = "Потянуть дверцу";

    [Header("Runtime")]
    [SerializeField] private DoorHandleGameRunner activeGame;

    public string InteractionText => interactionText;
    public bool isBlockInteract { get; set; }
    public event Action OnInteract;

    void OnEnable()
    {
        miniGameConfig.SetMiniGameLogic(this);
        CreateCanvas();
    }

    private void CreateCanvas()
    {
        var playerCanvas = GameObject.Find("PlayerUI canvas");
        if (playerCanvas != null && canvasPrefab != null)
        {
            newCanvas = Instantiate(canvasPrefab, playerCanvas.transform);
            newCanvas.transform.SetSiblingIndex(playerCanvas.transform.childCount - 2);
            uiContainer = newCanvas.GetComponent<CanvasGroup>();
            handleArea = newCanvas.transform.Find("HandleArea")?.GetComponent<RectTransform>();
            doorImage = handleArea.GetComponentInChildren<Image>();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBlockInteract)
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
        }
    }

    public void Interact()
    {
        if (isBlockInteract) return;

        // Создаем новый экземпляр раннера если его нет
        if (activeGame == null)
        {
            var runnerObj = new GameObject("DoorGameRunner");
            runnerObj.transform.SetParent(transform);
            activeGame = runnerObj.AddComponent<DoorHandleGameRunner>();
            activeGame.OnGameComplete += HandleGameComplete;
        }

        // Настраиваем игру
        activeGame.config = miniGameConfig;
        activeGame.uiContainer = uiContainer;
        activeGame.doorImage = doorImage;
        activeGame.handleArea = handleArea;
        activeGame.enabled = true;
        activeGame.Initialize();

        OnInteract?.Invoke();
    }

    private void HandleGameComplete()
    {
        // Уничтожаем Canvas при завершении игры
        if (newCanvas != null)
        {
            Destroy(newCanvas);
            newCanvas = null;
        }

        // Очищаем ссылку на раннер
        if (activeGame != null)
        {
            activeGame.OnGameComplete -= HandleGameComplete;
            Destroy(activeGame.gameObject);
            activeGame = null;
        }
    }

    public void BlockInteraction()
    {
        isBlockInteract = true;
    }

    private void OnDisable()
    {
        if (newCanvas != null)
        {
            Destroy(newCanvas);
        }
    }
}