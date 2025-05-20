using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField] private RectTransform buttonsPanel;
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button exitButton;

    [Header("Arrow Settings")]
    [SerializeField] private RectTransform selectionArrow;
    [SerializeField] private float arrowXOffset = -40f;
    [SerializeField] private float arrowYOffset = 0f;

    private LoadingScreen loadingScreen;

    private void Start()
    {
        if (selectionArrow == null)
        {
            Debug.LogError("Selection Arrow not assigned!");
            return;
        }

        InitializeButton(startButton, StartGame);
        InitializeButton(continueButton, ContinueGame);
        InitializeButton(optionsButton, OpenOptions);
        InitializeButton(exitButton, ExitGame);

        selectionArrow.gameObject.SetActive(false);
        loadingScreen = GetComponent<LoadingScreen>();
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

        // Событие при наведении
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => ShowArrow(button.GetComponent<RectTransform>()));
        trigger.triggers.Add(pointerEnter);

        // Событие при уходе курсора
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

    private void StartGame()
    {
        loadingScreen.ShowLoadingScreen("Main game");
    }

    private void ContinueGame()
    {
        Debug.Log("Continue Game");
    }

    private void OpenOptions()
    {
        Debug.Log("Options Menu");
    }

    private void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}