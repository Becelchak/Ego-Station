using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using TMPro;

public class ButtonsPuzzleUI : MonoBehaviour
{
    [SerializeField] private Button redButton;
    [SerializeField] private Button blueButton;
    [SerializeField] private Button greenButton;
    [SerializeField] private TextMeshProUGUI poemText;


    public delegate void OnPuzzleCompleted(bool success);
    public delegate void OnPuzzleFailed(bool fail);
    public event OnPuzzleCompleted PuzzleCompleted;
    public event OnPuzzleFailed PuzzleFailed;

    private List<Button> correctSequence = new List<Button>();
    private List<Button> playerSequence = new List<Button>();

    public void Initialize()
    {
        // ������������� ����� �������������
        poemText.text = "����� ��������� ����� ���� ������ � ������,\r\n����� ����� ������ ������ ����� �����,\r\n����� ���� ������ ��� ����� ����� �������";

        // ���������� ���������� ������������������
        correctSequence.Add(redButton);
        correctSequence.Add(blueButton);
        correctSequence.Add(greenButton);

        // ����������� ������
        redButton.onClick.AddListener(() => ButtonPressed(redButton));
        blueButton.onClick.AddListener(() => ButtonPressed(blueButton));
        greenButton.onClick.AddListener(() => ButtonPressed(greenButton));

        // ������� ������������������ ������
        playerSequence.Clear();
    }

    public void ButtonPressed(Button pressedButton)
    {
        playerSequence.Add(pressedButton);

        // ��������� ������������������
        if (playerSequence.Count == correctSequence.Count)
        {
            bool isCorrect = true;
            for (int i = 0; i < correctSequence.Count; i++)
            {
                if (playerSequence[i] != correctSequence[i])
                {
                    isCorrect = false;
                    break;
                }
            }

            if (isCorrect)
            {
                PuzzleCompleted.Invoke(true);
            }
            else
            {
                PuzzleFailed.Invoke(true);
            }
        }
        else if (playerSequence.Count > correctSequence.Count)
        {
            PuzzleFailed.Invoke(true);
        }
    }
}