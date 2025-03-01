public interface IDialog : IGlobalSubscriber
{
    /// <summary>
    /// �����, �������� ������� ����� �� ��������� � ������
    /// </summary>
    //void ChangeDialogPhrase();
    ///// <summary>
    ///// ���������� �������� ������� � ��������� �������� ��� ������������ ����������
    ///// </summary>
    ///

    void GoToNextPhrase();

    void ShowCurrentPhrase();
    void Disable();

    void ChangeDialogData(DialogData newDialog);

    /// <summary>
    /// ��������, �����������, ������������ �� ������.
    /// </summary>
    bool IsContinuesDialog { get; }
}
