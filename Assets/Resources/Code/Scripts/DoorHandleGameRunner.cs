using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DoorHandleGameRunner : MonoBehaviour
{
    public event Action OnGameComplete;

    [Header("References")]
    public DoorHandleMiniGame config;
    public CanvasGroup uiContainer;
    public Image doorImage;
    public RectTransform handleArea;
    public Image handleImage;

    // Input actions
    private InputAction mouseClickAction;
    private InputAction mousePositionAction;

    private bool isDragging;
    private Vector2 dragStartPosition;
    private float currentDragDistance;
    private float reboundProgress;

    private void Awake()
    {
        mouseClickAction = InputSystem.actions.FindAction("Handle");
        mousePositionAction = InputSystem.actions.FindAction("MousePosition");
    }

    public void Initialize()
    {
        // Настройка callbacks
        mouseClickAction.performed += OnMouseDown;
        mouseClickAction.canceled += OnMouseUp;
        mouseClickAction.Enable();
        mousePositionAction.Enable();

        ResetGame();
    }

    private void OnDisable()
    {
        if (mouseClickAction != null)
        {
            mouseClickAction.performed -= OnMouseDown;
            mouseClickAction.canceled -= OnMouseUp;
            mouseClickAction.Disable();
        }

        mousePositionAction?.Disable();
    }

    private void OnMouseDown(InputAction.CallbackContext context)
    {
        Vector2 mousePos = mousePositionAction.ReadValue<Vector2>();
        if (RectTransformUtility.RectangleContainsScreenPoint(handleArea, mousePos))
        {
            isDragging = true;
            dragStartPosition = mousePos;

            // Визуальная обратная связь
            if (handleImage != null)
                handleImage.color = new Color(0.9f, 0.9f, 0.9f);
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            var currentMousePos = mousePositionAction.ReadValue<Vector2>();
            var newX = Mathf.Clamp(
                dragStartPosition.x - currentMousePos.x,
                0,
                config.maxDragDistance);

            handleArea.anchoredPosition = new Vector2(-newX, 0);
            currentDragDistance = newX;
        }
        else if (reboundProgress < 1f)
        {
            reboundProgress += Time.deltaTime / config.reboundDuration;
            float easedProgress = config.reboundCurve.Evaluate(reboundProgress);
            handleArea.anchoredPosition = Vector2.Lerp(
                handleArea.anchoredPosition,
                Vector2.zero,
                easedProgress);
        }
    }

    private void OnMouseUp(InputAction.CallbackContext context)
    {
        if (!isDragging) return;

        var currentMousePos = mousePositionAction.ReadValue<Vector2>();
        var speed = (currentMousePos - (Vector2)handleArea.position).magnitude / Time.deltaTime;

        Debug.Log($"{speed} and config {config.maxSpeedThreshold}");
        if (speed > config.maxSpeedThreshold || currentDragDistance < config.maxDragDistance)
        {
            StartRebound();
        }
        else if (currentDragDistance >= config.maxDragDistance)
        {
            CompleteGame();
        }

        isDragging = false;
    }

    private void StartRebound()
    {
        reboundProgress = 0f;
        config.dialog.SetCurrentPhrase(config.onFailPhrase);
        config.dialog.ShowCurrentPhrase();
    }

    private void CompleteGame()
    {
        uiContainer.alpha = 0;
        uiContainer.blocksRaycasts = false;
        uiContainer.interactable = false;
        enabled = false;

        config.dialog.SetCurrentPhrase(config.onSuccessPhrase);
        config.dialog.ShowCurrentPhrase();

        OnGameComplete?.Invoke();
    }

    private void ResetGame()
    {
        doorImage.rectTransform.anchoredPosition = Vector2.zero;
        handleArea.anchoredPosition = Vector2.zero;
        isDragging = false;
        currentDragDistance = 0f;
        reboundProgress = 1f;

        uiContainer.alpha = 1;
        uiContainer.blocksRaycasts = true;
        uiContainer.interactable = true;
    }

    private void OnDestroy()
    {
        OnGameComplete = null;
    }
}