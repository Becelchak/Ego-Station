using AYellowpaper.SerializedCollections;
using EventBusSystem;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

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
    private Queue<Phrase> dialogStack = new Queue<Phrase>();
    private AudioSource audioSource;
    public bool IsContinuesDialog { get; private set; }
    protected bool isAdminAccsessEnable;

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
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.Freeze());
        dialogUI.SetDialogLogic(this);
        currentPhrase = dialog.startPhrase;
        IsContinuesDialog = true;
        ShowCurrentPhrase();
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

        if(audioSource.clip != null)
            audioSource.Play();

        dialogStack.Enqueue(currentPhrase);
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

        if (currentPhrase.NextPhrase != null)
        {
            currentPhrase = currentPhrase.NextPhrase;
            ShowCurrentPhrase();
        }
        else if(currentPhrase.DialogEvent != null)
        {
            // Set DialogLogic for FindInTable Event
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

    public void SetCurrentPhrase(Phrase currentPhrase)
    {
        if (currentPhrase == null)
        {
            EndDialog();
            return;
        }

        dialog.startPhrase = currentPhrase;
        StartDialog();
    }

    public void OnChoiceSelected(Choice choice)
    {
        var choiceNextPhrase = choice.GetNextPhrase();

        //// Set DialogLogic for FindInTable Event
        //if (choice.DialogEventDefault is DialogEvent Event)
        //{
        //    Event.SetDialogLogic(this);
        //}

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
        // Set DialogLogic for FindInTable Event
        if (choice.DialogEventDefault is DialogEvent Event)
        {
            Event.SetDialogLogic(this);
        }
    }

    public void ReturnToPreviousState()
    {
        if (dialogStack.Count > 0)
        {
            currentPhrase = dialogStack.Peek();
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
        dialogStack.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Dialog trigger");
        if (other.tag == "Player")
        {
            IsContinuesDialog = true;
            StartDialog();
        }
            
    }

    void Update()
    {
        if (dialogAction.WasPressedThisFrame() && IsContinuesDialog && !currentPhrase.IsChoise)
        {
            GoToNextPhrase();
        }

        //Admin accses actions

        if (adminAccsessAction.WasPressedThisFrame() && IsContinuesDialog)
        {
            isAdminAccsessEnable = !isAdminAccsessEnable;
            Debug.Log($"Admin accsess {isAdminAccsessEnable}");
        }

        if(isAdminAccsessEnable && IsContinuesDialog && previousDialogStates.WasPressedThisFrame())
        {
            Debug.Log($"Предыдущий граф диалога");
            ReturnToPreviousState();
        }
        else if (isAdminAccsessEnable && IsContinuesDialog && nextDialogStates.WasPressedThisFrame())
        {
            Debug.Log($"Следующий граф диалога");
            GoToNextPhrase();
        }

    }
}
