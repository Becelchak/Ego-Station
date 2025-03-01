using AYellowpaper.SerializedCollections;
using EventBusSystem;
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
    //private int phraseCounter = 0;
    //private List<Phrase> phrases = new List<Phrase>();

    private Phrase currentPhrase;
    private Queue<Phrase> dialogStack = new Queue<Phrase>();
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
        dialogUI.SetDialogLogic(this);
    }

    public void StartDialog()
    {
        if (dialog == null || dialog.startPhrase == null)
        {
            Debug.LogError("������ �� ��������!");
            EndDialog();
            return;
        }
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.Freeze());
        currentPhrase = dialog.startPhrase;
        IsContinuesDialog = true;
        ShowCurrentPhrase();
    }

    /// <summary>
    /// ���������� ������� ����� � ������������ ������, ���� ��� ����.
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

        //if (currentPhrase.IsChoise && currentPhrase.Choises != null && currentPhrase.Choises.Count > 0)
        //{
        //    dialogStack.Push(currentPhrase);
        //}
        dialogStack.Enqueue(currentPhrase);
    }

    /// <summary>
    /// ��������� � ��������� �����.
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
        else
        {
            EndDialog();
        }
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

    public void ReturnToPreviousState()
    {
        if (dialogStack.Count > 0)
        {
            currentPhrase = dialogStack.Peek();
            ShowCurrentPhrase();
        }
        else
        {
            Debug.Log("��� ���������� ��������� ��� ��������.");
        }
    }


    //public void ChangeDialogPhrase()
    //{
    //    phrases = dialog.GetPhrases();
    //    if(phraseCounter < phrases.Count)
    //    {
    //        var character = characters[phrases[phraseCounter].CharacterName];
    //        dialogUI.PrepareDialogPanel(character, phrases[phraseCounter]);
    //    }
    //    else
    //    {
    //        EndDialog();

    //    }

    //}

    private void EndDialog()
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

    public void ChangeDialogData(DialogData newDialog)
    {
        dialog = newDialog;
        dialogUI.EndDialog();
        //ChangeDialogPhrase();
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
            Debug.Log($"���������� ���� �������");
            ReturnToPreviousState();
        }
        else if (isAdminAccsessEnable && IsContinuesDialog && nextDialogStates.WasPressedThisFrame())
        {
            Debug.Log($"��������� ���� �������");
            GoToNextPhrase();
        }

    }
}
