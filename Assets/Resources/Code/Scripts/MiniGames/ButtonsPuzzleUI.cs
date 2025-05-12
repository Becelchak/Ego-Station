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
        // Устанавливаем текст стихотворения
        poemText.text = "Когда аварийный пульс алый стучит в висках,\r\nКогда холод разума глушит синий экран,\r\nКогда лишь зелёный луч маяка сулит возврат";

        // Определяем правильную последовательность
        correctSequence.Add(redButton);
        correctSequence.Add(blueButton);
        correctSequence.Add(greenButton);

        // Настраиваем кнопки
        redButton.onClick.AddListener(() => ButtonPressed(redButton));
        blueButton.onClick.AddListener(() => ButtonPressed(blueButton));
        greenButton.onClick.AddListener(() => ButtonPressed(greenButton));

        // Очищаем последовательность игрока
        playerSequence.Clear();
    }

    public void ButtonPressed(Button pressedButton)
    {
        playerSequence.Add(pressedButton);

        // Проверяем последовательность
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