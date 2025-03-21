using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RiddleData;

public class RiddlesMiniGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI riddleText; // Текст загадки
    [SerializeField] private GameObject answerButtonPrefab; // Префаб кнопки ответа
    [SerializeField] private Transform answersContainer; // Контейнер для кнопок ответов
    [SerializeField] private TextMeshProUGUI errorCounterText; // Текст счетчика ошибок

    private RiddleData riddleData; // Данные для мини-игры
    private RiddlesLogic miniGameLogic; // Логика мини-игры
    private int currentRiddleIndex = 0; // Индекс текущей загадки
    private int errorCount = 0; // Счетчик ошибок
    private int correctCount = 0;

    public void Initialize(RiddleData data, RiddlesLogic logic)
    {
        riddleData = data;
        miniGameLogic = logic;
        ShowNextRiddle();
    }

    private void ShowNextRiddle()
    {
        if (currentRiddleIndex >= riddleData.Riddles.Count)
        {
            // Мини-игра завершена
            miniGameLogic.OnMiniGameCompleted(errorCount, correctCount);
            return;
        }

        // Очистка контейнера от предыдущих кнопок
        foreach (Transform child in answersContainer)
        {
            Destroy(child.gameObject);
        }

        // Отображение текущей загадки
        Riddle currentRiddle = riddleData.Riddles[currentRiddleIndex];
        riddleText.text = currentRiddle.Question;

        // Создание кнопок для ответов
        for (int i = 0; i < currentRiddle.Answers.Count; i++)
        {
            // Создаем кнопку из префаба
            GameObject buttonInstance = Instantiate(answerButtonPrefab, answersContainer);
            buttonInstance.SetActive(true);

            // Настраиваем текст кнопки
            var buttonText = buttonInstance.GetComponentInChildren<Text>();
            buttonText.text = currentRiddle.Answers[i].Text;

            // Настраиваем обработчик нажатия
            Button button = buttonInstance.GetComponent<Button>();
            int answerIndex = i; // Локальная копия для замыкания
            button.onClick.AddListener(() => OnAnswerSelected(answerIndex));
        }

        // Обновление счетчика ошибок
        errorCounterText.text = $"Ошибки: {errorCount}";
    }

    private void OnAnswerSelected(int answerIndex)
    {
        Riddle currentRiddle = riddleData.Riddles[currentRiddleIndex];

        if (!currentRiddle.Answers[answerIndex].IsCorrect)
        {
            errorCount++;
            errorCounterText.text = $"Ошибки: {errorCount}";
        }
        else
            correctCount++;

        currentRiddleIndex++;
        ShowNextRiddle();
    }
}
