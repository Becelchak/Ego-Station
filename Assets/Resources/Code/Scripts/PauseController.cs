using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private RectTransform buttonsPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    [Header("Arrow Settings")]
    [SerializeField] private RectTransform selectionArrow;
    [SerializeField] private float arrowXOffset = -165f;
    [SerializeField] private float arrowYOffset = 0f;

    private CanvasGroup pauseCanvasGroup;
    private InputAction pauseAction;
    private bool pause = false;

    private void Start()
    {
        if (selectionArrow == null)
        {
            Debug.LogError("Selection Arrow not assigned!");
            return;
        }
        InitializeButton(continueButton, ContinueGame);
        InitializeButton(optionsButton, OpenOptions);
        InitializeButton(exitButton, ExitGame);

        selectionArrow.gameObject.SetActive(false);
        pauseCanvasGroup = GetComponent<CanvasGroup>();
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    private void Update()
    {
        if (pauseAction.WasPressedThisFrame() && !pause)
        {
            OpenPauseMenu();
            pause = true;
        }
        else if (pauseAction.WasPressedThisFrame() && pause)
        {
            ContinueGame();
            pause = false;
        }
    }

    private void InitializeButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button == null) return;

        button.onClick.AddListener(action);
        AddHoverEffects(button);
    }

    private void AddHoverEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => ShowArrow(button.GetComponent<RectTransform>()));
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => HideArrow());
        trigger.triggers.Add(pointerExit);
    }

    private void ShowArrow(RectTransform buttonRect)
    {
        selectionArrow.gameObject.SetActive(true);

        Vector3 targetPosition = buttonRect.position;
        targetPosition.x += arrowXOffset;
        targetPosition.y += arrowYOffset;

        selectionArrow.position = targetPosition;
        selectionArrow.SetParent(buttonsPanel, true);
    }

    private void HideArrow()
    {
        selectionArrow.gameObject.SetActive(false);
    }

    private void ContinueGame()
    {
        pauseCanvasGroup.alpha = 0.0f;
        pauseCanvasGroup.blocksRaycasts = false;
        pauseCanvasGroup.interactable = false;

        Time.timeScale = 1f;
    }

    private void OpenOptions()
    {
        Debug.Log("Options Menu");
    }

    private void ExitGame()
    {
        SceneManager.LoadScene("Main menu");
    }

    private void OpenPauseMenu()
    {
        Time.timeScale = 0f;

        pauseCanvasGroup.alpha = 1.0f;
        pauseCanvasGroup.blocksRaycasts = true;
        pauseCanvasGroup.interactable = true;
    }
}
