public interface IDialog : IGlobalSubscriber
{
    /// <summary>
    /// Метод, меняющий текущую фразу на следующую в списке
    /// </summary>
    //void ChangeDialogPhrase();
    ///// <summary>
    ///// Выключение триггера диалога и обнуление счетчика для последующего интерфейса
    ///// </summary>
    ///

    void GoToNextPhrase();

    void ShowCurrentPhrase();
    void Disable();

    void ChangeDialogData(DialogData newDialog);

    /// <summary>
    /// Свойство, указывающее, продолжается ли диалог.
    /// </summary>
    bool IsContinuesDialog { get; }
}
