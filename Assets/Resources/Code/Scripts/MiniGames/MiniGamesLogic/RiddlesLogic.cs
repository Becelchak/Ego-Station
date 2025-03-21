using EventBusSystem;
using UnityEngine;

public class RiddlesLogic : Logic, IDialogEventSubscriber
{
    [SerializeField] private RiddlesMiniGame riddleMiniGamePrefab;
    [SerializeField] private RiddleData riddleData;

    private RiddlesMiniGame currentMiniGame;
    private DialogLogic dialogLogic;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void OnStartRiddleMiniGame(DialogLogic dialogLogic)
    {
        StartMiniGame(dialogLogic);
    }

    public void StartMiniGame(DialogLogic dialogLogic)
    {
        this.dialogLogic = dialogLogic;

        // Создаем экземпляр мини-игры
        currentMiniGame = Instantiate(riddleMiniGamePrefab);
        currentMiniGame.Initialize(riddleData, this);
    }

    public void OnMiniGameCompleted(int errorCount, int correctCount)
    {
        // Логика завершения мини-игры
        Debug.Log($"Мини-игра завершена с {errorCount} ошибками.");

        // Вызов DialogEvent в зависимости от количества ошибок
        if (errorCount == 0 || correctCount > errorCount)
        {
            dialogLogic.SetCurrentPhrase(riddleData.SuccessPhrase);
        }
        else
        {
            dialogLogic.SetCurrentPhrase(riddleData.FailPhrase);
        }

        Destroy(currentMiniGame.gameObject);
    }
}
