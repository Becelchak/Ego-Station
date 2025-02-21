public interface IDialog : IGlobalSubscriber
{
    void ChangeDialogPhrase();
    void Disable();

    void ChangeDialogData(DialogData newDialog);

    /// <summary>
    /// ��������, �����������, ������������ �� ������.
    /// </summary>
    bool IsContinuesDialog { get; }
}
