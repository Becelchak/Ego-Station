public interface IDialog : IGlobalSubscriber
{
    void ChangeDialogPhrase();
    void Disable();

    void ChangeDialogData(DialogData newDialog);

    /// <summary>
    /// —войство, указывающее, продолжаетс€ ли диалог.
    /// </summary>
    bool IsContinuesDialog { get; }
}
