using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialog Events/ FindInTable")]
public class FindInTable : DialogEvent
{
    [SerializeField] private CanvasGroup uiTable;
    [SerializeField] private List<Phrase> onToggleMiniGamePhrase = new();
    private int counter;
    private int totalItems;
    private bool isMiniGameActive = false;
    private InputAction toggleMiniGameAction;
    private DialogLogic dialogLogic;
    public override void Raise()
    {
        InitializeMiniGame();
    }

    private void InitializeMiniGame()
    {
        counter = 0;

        uiTable = GameObject.Find("FindTable canvas").GetComponent<CanvasGroup>();
        uiTable.alpha = 1;
        uiTable.blocksRaycasts = true;
        uiTable.interactable = true;

        var buttons = uiTable.transform.GetComponentsInChildren<Button>();
        totalItems = buttons.Length;

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => { FindItem(button); });
        }

        toggleMiniGameAction = InputSystem.actions.FindAction("Cancel");
        if (toggleMiniGameAction != null)
        {
            toggleMiniGameAction.performed += OnToggleMiniGame;
            toggleMiniGameAction.Enable();
        }
        else
        {
            Debug.LogError("Input Action 'Cancel' �� ������!");
        }

        isMiniGameActive = true;
    }

    private void OnToggleMiniGame(InputAction.CallbackContext context)
    {
        if (isMiniGameActive)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;
            isMiniGameActive = false;

            if (dialogLogic != null && onToggleMiniGamePhrase != null)
            {
                var rndPhraseIndex = Random.Range(0, onToggleMiniGamePhrase.Count-1);
                dialogLogic.SetCurrentPhrase(onToggleMiniGamePhrase[rndPhraseIndex]);
            }

            Debug.Log("����-���� ��������. �� ������ ���������� ������.");
        }
        else
        {
            uiTable.alpha = 1;
            uiTable.blocksRaycasts = true;
            uiTable.interactable = true;
            isMiniGameActive = true;

            Debug.Log("����-���� ����������. ����������� �����.");
        }
    }


    public void FindItem(Button button)
    {
        counter++;

        Destroy(button.gameObject);

        Debug.Log($"������� {counter} �� {totalItems} ����������");

        if (counter == totalItems)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;

            OnDisable();
            Debug.Log("�� ������� �������!");
        }
    }

    private void OnDisable()
    {
        if (toggleMiniGameAction != null)
        {
            toggleMiniGameAction.performed -= OnToggleMiniGame;
            toggleMiniGameAction.Disable();
        }
    }
    /// <summary>
    /// ������������� ������ �� DialogLogic ��� ����������� �������.
    /// </summary>
    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}
