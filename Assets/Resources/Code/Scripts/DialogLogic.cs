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
    private List<Phrase> dialogHistory = new List<Phrase>(); // ����� ������ ��� ������� �������
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
            Debug.LogError("������ �� ��������!");
            EndDialog();
            return;
        }

        if (!CheckRequirements())
        {
            Debug.Log("���������� ��� ������ ������� �� ���������.");
            return;
        }

        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.Freeze());
        dialogUI.SetDialogLogic(this);
        // ���������� ����������� ���� ������� ��� ������ �������
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

        // ���������� ���������� ��� ���� ��������� ����
        ResetAllChoicesAvailability(phrase.NextPhrase);
    }

    /// <summary>
    /// ���������, ������������� �� ����� ����������� ��� ������ �������.
    /// </summary>
    private bool CheckRequirements()
    {
        if (dialog.requirements == null || dialog.requirements.Count == 0)
        {
            return true; // ���� ���������� ���, ������ ����� ������
        }

        PlayerData playerData = GameManager.Instance.playerData;
        if (playerData == null)
        {
            Debug.LogError("PlayerData �� ������!");
            return false;
        }

        foreach (var requirement in dialog.requirements)
        {
            if (playerData.GetItemQuantity(requirement.itemId) < requirement.requiredQuantity)
            {
                Debug.Log($"�� ������� �������� {requirement.itemId}. ���������: {requirement.requiredQuantity}, ����: {playerData.GetItemQuantity(requirement.itemId)}");
                return false;
            }
        }

        return true;
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
        audioSource.Stop();
        audioSource.clip = character.GetSpeachSound();

        if (audioSource.clip != null)
            audioSource.Play();

        // ��������� ����� � ������� ������ ���� ��� �� ��������� �����
        if (currentHistoryIndex == -1 || dialogHistory[currentHistoryIndex] != currentPhrase)
        {
            // ���� �� �� � ����� �������, ������� ��� ����� �������� �������
            if (currentHistoryIndex < dialogHistory.Count - 1)
            {
                dialogHistory.RemoveRange(currentHistoryIndex + 1, dialogHistory.Count - currentHistoryIndex - 1);
            }

            dialogHistory.Add(currentPhrase);
            currentHistoryIndex = dialogHistory.Count - 1;
        }
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
    /// ������������� ������� ����� � ���������� ������ � ���� �����.
    /// </summary>
    /// <param name="currentPhrase">�����, � ������� ����� ���������� ������.</param>
    public void SetCurrentPhrase(Phrase newPhrase)
    {
        if (newPhrase == null)
        {
            Debug.LogError("���������� ����� ����� null!");
            EndDialog();
            return;
        }

        // ��������� ������� �����
        currentPhrase = newPhrase;

        // ������� ���� �������, ����� �������� �������� ��� �������� � ���������� ����������
        //dialogStack.Clear();

        // ���������� ������ � ����� �����
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
            Debug.Log("��� ���������� ��������� ��� ��������.");
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
                Debug.Log("���������� ���� �������");
                ReturnToPreviousState();
            }
            else if (nextDialogStates.WasPressedThisFrame())
            {
                Debug.Log("��������� ���� �������");
                GoToNextState();
            }
        }
    }
}