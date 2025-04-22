using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EventBusSystem;
using System;
using System.Linq;

public class GeneratorConsole : MonoBehaviour, IInteractive
{
    [Header("Game Settings")]
    [SerializeField] private int sequenceLength = 4;
    [SerializeField] private float buttonHighlightTime = 0.5f;
    [SerializeField] private float delayBetweenButtons = 0.3f;

    [Header("UI References")]
    [SerializeField] private GameObject miniGameCanvas;
    [SerializeField] private List<Button> gameButtons;
    [SerializeField] private List<Image> buttonLights = new();
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color inactiveColor = Color.red;
    [Header("Game Settings")]
    [SerializeField] private GameObject dialog;
    //private BoxCollider2D dialogCollider;

    private GameObject instanceMiniGame;
    private List<int> currentSequence = new List<int>();
    private int currentStep = 0;
    private bool isGameActive = false;
    private bool isShowingSequence = false;
    private Animator animator;
    private DialogLogic dialogLogic;

    [SerializeField] private bool _isBlockInteract;
    public bool isBlockInteract { get => _isBlockInteract; set => _isBlockInteract = value; }
    public string InteractionText => "Открыть консоль";
    public event Action OnInteract;

    private void InitializeButtons(GameObject canvasWithButtons)
    {
        gameButtons = canvasWithButtons.GetComponentsInChildren<Button>().ToList();
        foreach (var button in gameButtons) 
        {
            buttonLights.Add(button.GetComponentInChildren<Image>());
        }
        for (int i = 0; i < gameButtons.Count; i++)
        {
            int buttonIndex = i;
            gameButtons[i].onClick.AddListener(() => OnButtonPressed(buttonIndex));
            buttonLights[i].color = inactiveColor;
        }
    }

    public void Interact()
    {
        if (isBlockInteract || isGameActive) return;

        dialog.SetActive(true);
        //dialogCollider = dialog.GetComponent<BoxCollider2D>();
        dialogLogic = dialog.GetComponent<DialogLogic>();

        var playerCanvas = GameObject.Find("PlayerUI canvas");
        instanceMiniGame = Instantiate(miniGameCanvas, playerCanvas.transform);
        instanceMiniGame.transform.SetSiblingIndex(playerCanvas.transform.childCount - 3);
        InitializeButtons(instanceMiniGame);
        animator = GetComponentInParent<Animator>();

        StartMiniGame();
        OnInteract?.Invoke();
    }

    private void StartMiniGame()
    {
        isGameActive = true;
        miniGameCanvas.SetActive(true);
        GenerateNewSequence();
        StartCoroutine(ShowSequence());
    }

    private void GenerateNewSequence()
    {
        currentSequence.Clear();
        for (int i = 0; i < sequenceLength; i++)
        {
            currentSequence.Add(UnityEngine.Random.Range(0, gameButtons.Count));
        }
    }

    private IEnumerator ShowSequence()
    {
        isShowingSequence = true;
        DisableAllButtons();

        yield return new WaitForSeconds(0.5f);

        foreach (int buttonIndex in currentSequence)
        {
            HighlightButton(buttonIndex);
            yield return new WaitForSeconds(buttonHighlightTime);
            UnhighlightButton(buttonIndex);
            yield return new WaitForSeconds(delayBetweenButtons);
        }

        EnableAllButtons();
        isShowingSequence = false;
        currentStep = 0;
    }

    private void OnButtonPressed(int buttonIndex)
    {
        if (!isGameActive || isShowingSequence) return;

        if (buttonIndex == currentSequence[currentStep])
        {
            // Правильное нажатие
            HighlightButton(buttonIndex);
            currentStep++;

            if (currentStep >= currentSequence.Count)
            {
                // Последовательность завершена
                StartCoroutine(CompleteGame());
            }
        }
        else
        {
            // Неправильное нажатие
            StartCoroutine(WrongInput());
        }
    }

    private IEnumerator CompleteGame()
    {
        DisableAllButtons();
        yield return new WaitForSeconds(0.2f);

        // Мини-игра пройдена
        Debug.Log("Mini-game completed!");
        isBlockInteract = true;
        animator.SetTrigger("GeneratorOn");

        dialogLogic.SetCurrentPhrase(dialogLogic.GetDialogData().GetPhrases()[1]);
        dialogLogic.ShowCurrentPhrase();
        //dialogCollider.enabled = true;
        EventBus.RaiseEvent<IGeneratorSubscriber>(s => s.OnGeneratorActivated());

        CloseMiniGame();
    }

    private IEnumerator WrongInput()
    {
        isGameActive = false;
        dialogLogic.SetCurrentPhrase(dialogLogic.GetDialogData().GetPhrases()[2]);
        dialogLogic.ShowCurrentPhrase();
        DisableAllButtons();

        // Мигаем красным при ошибке
        for (int i = 0; i < 3; i++)
        {
            foreach (var light in buttonLights)
            {
                light.color = Color.red;
            }
            yield return new WaitForSeconds(0.2f);

            foreach (var light in buttonLights)
            {
                light.color = inactiveColor;
            }
            yield return new WaitForSeconds(0.2f);
        }

        // Перезапускаем игру
        StartMiniGame();
    }

    private void HighlightButton(int index)
    {
        buttonLights[index].color = activeColor;
    }

    private void UnhighlightButton(int index)
    {
        buttonLights[index].color = inactiveColor;
    }

    private void EnableAllButtons()
    {
        foreach (var button in gameButtons)
        {
            button.interactable = true;
        }
    }

    private void DisableAllButtons()
    {
        foreach (var button in gameButtons)
        {
            button.interactable = false;
        }
        foreach (var light in buttonLights)
        {
            light.color = inactiveColor;
        }
    }

    private void CloseMiniGame()
    {
        isGameActive = false;
        Destroy(instanceMiniGame);
        DisableAllButtons();
    }

    public void BlockInteraction()
    {
        isBlockInteract = true;
        CloseMiniGame();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));

    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player" || isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }
}