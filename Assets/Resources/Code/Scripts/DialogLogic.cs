using AYellowpaper.SerializedCollections;
using EventBusSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class DialogLogic : MonoBehaviour, IDialog
{
    [SerializeField] private AYellowpaper.SerializedCollections.SerializedDictionary<string, Character> characters;
    [SerializeField] private DialogData dialog;
    private DialogManagerUI dialogUI;
    private InputAction dialogAction;
    private int phraseCounter = 0;
    private List<Phrase> phrases = new List<Phrase>();
    public bool IsContinuesDialog { get; private set; }

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
    }

    public void ChangeDialogPhrase()
    {
        phrases = dialog.GetPhrases();
        if(phraseCounter < phrases.Count)
        {
            var character = characters[phrases[phraseCounter].characterName];
            dialogUI.PrepareDialogPanel(character, phrases[phraseCounter]);
        }
        else
        {
            dialogUI.EndDialog();
            // Отключить триггер диалога
            Disable();

        }

    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Dialog trigger");
        if (other.tag == "Player")
        {
            IsContinuesDialog = true;
            ChangeDialogPhrase();
        }
            
    }

    void Update()
    {
        if (dialogAction.WasPressedThisFrame() && IsContinuesDialog)
        {
            phraseCounter++;
            ChangeDialogPhrase();
        }

    }

    public void Disable()
    {
        GetComponent<Collider>().enabled = false;
        IsContinuesDialog = false;
    }

    public void ChangeDialogData(DialogData newDialog)
    {
        dialog = newDialog;
    }
}
