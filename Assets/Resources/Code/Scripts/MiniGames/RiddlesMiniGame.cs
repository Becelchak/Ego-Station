using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RiddleData;

public class RiddlesMiniGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI riddleText;
    [SerializeField] private GameObject answerButtonPrefab;
    [SerializeField] private Transform answersContainer;
    [SerializeField] private TextMeshProUGUI errorCounterText;

    private RiddleData riddleData;
    private RiddlesLogic miniGameLogic;
    private int currentRiddleIndex = 0;
    private int errorCount = 0;
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

            miniGameLogic.OnMiniGameCompleted(errorCount, correctCount);
            return;
        }


        foreach (Transform child in answersContainer)
        {
            Destroy(child.gameObject);
        }


        Riddle currentRiddle = riddleData.Riddles[currentRiddleIndex];
        riddleText.text = currentRiddle.Question;


        for (int i = 0; i < currentRiddle.Answers.Count; i++)
        {

            GameObject buttonInstance = Instantiate(answerButtonPrefab, answersContainer);
            buttonInstance.SetActive(true);

            var buttonText = buttonInstance.GetComponentInChildren<Text>();
            buttonText.text = currentRiddle.Answers[i].Text;

            Button button = buttonInstance.GetComponent<Button>();
            var answerIndex = i;
            button.onClick.AddListener(() => OnAnswerSelected(answerIndex));
        }

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
