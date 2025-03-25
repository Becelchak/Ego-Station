using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ButtonsPuzzle")]
public class ButtonsPuzzle : DialogEvent
{
    [SerializeField] private GameObject puzzleUIPrefab;
    [SerializeField] private Phrase successPhrase;
    [SerializeField] private Phrase failPhrase;

    private ButtonsPuzzleUI currentPuzzleUI;
    private DialogLogic dialogLogic;

    public override void Raise()
    {
        // ������� UI ����-����
        var puzzleUIObject = Instantiate(puzzleUIPrefab);
        currentPuzzleUI = puzzleUIObject.GetComponent<ButtonsPuzzleUI>();

        // ����������� ��������
        currentPuzzleUI.PuzzleCompleted += OnPuzzleCompleted;
        currentPuzzleUI.PuzzleFailed += OnPuzzleFailed;

        Debug.Log("���� �������");
        // �������������� ����-����
        currentPuzzleUI.Initialize();
    }

    private void OnPuzzleCompleted(bool succses)
    {
        Destroy(currentPuzzleUI.gameObject);
        dialogLogic.SetCurrentPhrase(successPhrase);
    }

    private void OnPuzzleFailed(bool fail)
    {
        Destroy(currentPuzzleUI.gameObject);
        dialogLogic.SetCurrentPhrase(failPhrase);
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}