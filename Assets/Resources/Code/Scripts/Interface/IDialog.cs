public interface IDialog : IGlobalSubscriber
{
    void ChangeDialogPhrase();
    void Disable();

    /// <summary>
    /// —войство, указывающее, продолжаетс€ ли диалог.
    /// </summary>
    bool IsContinuesDialog { get; }
}
