public interface IDialog : IGlobalSubscriber
{
    void ChangeDialogPhrase();
    void Disable();

    /// <summary>
    /// ��������, �����������, ������������ �� ������.
    /// </summary>
    bool IsContinuesDialog { get; }
}
