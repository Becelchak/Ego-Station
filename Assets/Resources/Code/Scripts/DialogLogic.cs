using EventBusSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogLogic : MonoBehaviour, IDialog
{
    [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<string, Character> characters;
    [SerializeField] private DialogData dialog;
    private DialogManagerUI dialogUI;
    private InputAction dialogAction;
    private InputAction adminAccsessAction;
    private InputAction previousDialogStates;
    private InputAction nextDialogStates;

    private Phrase currentPhrase;
    private List<Phrase> dialogHistory = new List<Phrase>(); // Новый список для истории диалога
    private int currentHistoryIndex = -1;
    private AudioSource audioSource;
    public bool IsContinuesDialog { get; private set; }
    protected bool isAdminAccsessEnable;
    public bool IsAdminAccsessEnable => isAdminAccsessEnable;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    void Start()
    {
        var obj = GameObject.Find("Dialog UI canvas");
        dialogUI = obj.GetComponent<DialogManagerUI>();
        dialogAction = InputSystem.actions.FindAction("DialogContinue");
        adminAccsessAction = InputSystem.actions.FindAction("ToggleAdminAccess");
        previousDialogStates = InputSystem.actions.FindAction("PreviousDialogStates");
        nextDialogStates = InputSystem.actions.FindAction("NextDialogStates");
        audioSource = GetComponent<AudioSource>();
    }

    public void StartDialog()
    {
        if (dialog == null || dialog.startPhrase == null)
        {
            Debug.LogError("Диалог не настроен!");
            EndDialog();
            return;
        }

        if (!CheckRequirements())
        {
            Debug.Log("Требования для начала диалога не выполнены.");
            return;
        }

        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.Freeze());
        dialogUI.SetDialogLogic(this);
        // Сбрасываем доступность всех выборов при начале диалога
        ResetAllChoicesAvailability(dialog.startPhrase);

        currentPhrase = dialog.startPhrase;
        IsContinuesDialog = true;
        ShowCurrentPhrase();
    }

    private void ResetAllChoicesAvailability(Phrase phrase)
    {
        if (phrase == null) return;

        if (phrase.IsChoise)
        {
            foreach (var choice in phrase.Choises)
            {
                choice.ResetAvailability();
            }
        }

        // Рекурсивно сбрасываем для всех следующих фраз
        ResetAllChoicesAvailability(phrase.NextPhrase);
    }

    /// <summary>
    /// Проверяет, удовлетворяет ли игрок требованиям для начала диалога.
    /// </summary>
    private bool CheckRequirements()
    {
        if (dialog.requirements == null || dialog.requirements.Count == 0)
        {
            return true; // Если требований нет, диалог можно начать
        }

        PlayerData playerData = GameManager.Instance.playerData;
        if (playerData == null)
        {
            Debug.LogError("PlayerData не найден!");
            return false;
        }

        foreach (var requirement in dialog.requirements)
        {
            if (playerData.GetItemQuantity(requirement.itemId) < requirement.requiredQuantity)
            {
                Debug.Log($"Не хватает предмета {requirement.itemId}. Требуется: {requirement.requiredQuantity}, есть: {playerData.GetItemQuantity(requirement.itemId)}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Отображает текущую фразу и обрабатывает выборы, если они есть.
    /// </summary>
    public void ShowCurrentPhrase()
    {
        if (currentPhrase == null)
        {
            EndDialog();
            return;
        }

        var character = characters[currentPhrase.CharacterName];
        dialogUI.PrepareDialogPanel(character, currentPhrase);
        audioSource.Stop();
        audioSource.clip = character.GetSpeachSound();

        if (audioSource.clip != null)
            audioSource.Play();

        // Добавляем фразу в историю только если это не повторный показ
        if (currentHistoryIndex == -1 || dialogHistory[currentHistoryIndex] != currentPhrase)
        {
            // Если мы не в конце истории, удаляем все после текущего индекса
            if (currentHistoryIndex < dialogHistory.Count - 1)
            {
                dialogHistory.RemoveRange(currentHistoryIndex + 1, dialogHistory.Count - currentHistoryIndex - 1);
            }

            dialogHistory.Add(currentPhrase);
            currentHistoryIndex = dialogHistory.Count - 1;
        }
    }

    /// <summary>
    /// Переходит к следующей фразе.
    /// </summary>
    public void GoToNextPhrase()
    {
        if (currentPhrase == null)
        {
            EndDialog();
            return;
        }

        if (currentPhrase.DialogEvent != null && currentPhrase.NextPhrase != null)
        {
            if (currentPhrase.DialogEvent is DialogEvent Event)
            {
                Event.SetDialogLogic(this);
            }
            currentPhrase.DialogEvent.Raise();
        }

        if (currentPhrase.NextPhrase != null)
        {
            currentPhrase = currentPhrase.NextPhrase;
            ShowCurrentPhrase();
        }
        else if (currentPhrase.DialogEvent != null)
        {
            // Set DialogLogic for  DialogEvent
            if (currentPhrase.DialogEvent is DialogEvent Event)
            {
                Event.SetDialogLogic(this);
            }
            currentPhrase.DialogEvent.Raise();
            EndDialog();
        }
        else
        {
            EndDialog();
        }
    }

    public void GoToNextState()
    {
        if (currentHistoryIndex < dialogHistory.Count - 1)
        {
            currentHistoryIndex++;
            currentPhrase = dialogHistory[currentHistoryIndex];
            ShowCurrentPhrase();
        }
        else if (currentPhrase.NextPhrase != null)
        {
            currentPhrase = currentPhrase.NextPhrase;
            ShowCurrentPhrase();
        }
    }


    /// <summary>
    /// Устанавливает текущую фразу и продолжает диалог с этой фразы.
    /// </summary>
    /// <param name="currentPhrase">Фраза, с которой нужно продолжить диалог.</param>
    public void SetCurrentPhrase(Phrase newPhrase)
    {
        if (newPhrase == null)
        {
            Debug.LogError("Переданная фраза равна null!");
            EndDialog();
            return;
        }

        // Обновляем текущую фразу
        currentPhrase = newPhrase;

        // Очищаем стек диалога, чтобы избежать путаницы при возврате к предыдущим состояниям
        //dialogStack.Clear();

        // Продолжаем диалог с новой фразы
        IsContinuesDialog = true;
        ShowCurrentPhrase();
    }

    public void SetDialogData(DialogData data)
    {
        dialog = data;
    }

    public void OnChoiceSelected(Choice choice)
    {
        var choiceNextPhrase = choice.GetNextPhrase();

        if (choiceNextPhrase != null)
        {
            currentPhrase = choiceNextPhrase;
            ShowCurrentPhrase();
        }
        else
        {
            EndDialog();
        }
    }

    public void SetLogicForEvent(Choice choice)
    {
        // Set DialogLogic for FindInTableMiniGame Event
        if (choice.DialogEventDefault is DialogEvent Event)
        {
            Event.SetDialogLogic(this);
        }
    }

    public void ReturnToPreviousState()
    {
        if (dialogHistory.Count > 1 && currentHistoryIndex > 0)
        {
            currentHistoryIndex--;
            currentPhrase = dialogHistory[currentHistoryIndex];
            ShowCurrentPhrase();
        }
        else
        {
            Debug.Log("Нет предыдущих состояний для возврата.");
        }
    }

    public void EndDialog()
    {
        dialogUI.EndDialog();
        Disable();
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.Unfreeze());
    }

    public void Disable()
    {
        GetComponent<Collider2D>().enabled = false;
        IsContinuesDialog = false;
        //dialogStack.Clear();
    }

    public DialogData GetDialogData() 
    {
        return dialog;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Dialog trigger");
        if (other.tag == "Player")
        {
            StartDialog();
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player" && !IsContinuesDialog)
        {
            StartDialog();
        }
    }

    void Update()
    {
        if (dialogAction.WasPressedThisFrame() && IsContinuesDialog && !currentPhrase.IsChoise)
        {
            GoToNextPhrase();
        }

        //Admin access actions
        if (adminAccsessAction.WasPressedThisFrame() && IsContinuesDialog)
        {
            isAdminAccsessEnable = !isAdminAccsessEnable;
            Debug.Log($"Admin access {isAdminAccsessEnable}");
        }

        if (isAdminAccsessEnable && IsContinuesDialog)
        {
            if (previousDialogStates.WasPressedThisFrame())
            {
                Debug.Log("Предыдущий граф диалога");
                ReturnToPreviousState();
            }
            else if (nextDialogStates.WasPressedThisFrame())
            {
                Debug.Log("Следующий граф диалога");
                GoToNextState();
            }
        }
    }
}