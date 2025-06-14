using EventBusSystem;
using UnityEngine;
using static PlayerManager;

[CreateAssetMenu(menuName = "Dialog/ Choice")]
public class Choice : ScriptableObject
{
    [SerializeField] private string text;
    public string Text => text;

    [Header("Events")]
    [SerializeField] protected DialogEvent dialogEventDefault;
    public DialogEvent DialogEventDefault => dialogEventDefault;
    [SerializeField] protected DialogEvent dialogEventSuccess;
    public DialogEvent DialogEventSuccess => dialogEventSuccess;
    [SerializeField] protected DialogEvent dialogEventFailed;
    public DialogEvent DialogEventFailed => dialogEventFailed;

    [Header("Attributes")]
    [SerializeField] private bool isCheckingChoice;
    public bool IsCheckingChoice => isCheckingChoice;
    [SerializeField] private PlayerAttributes checkAttribute;
    public PlayerAttributes CheckAttribute => checkAttribute;
    [SerializeField] private int difficultCheckAttribute;
    [Range(0, 20)]
    [SerializeField] private double rewardAttribute = 0;
    private bool resultCheckAttribute;
    //public bool ResultCheckAttribute => resultCheckAttribute;

    [Header("Next Phrase")]
    [SerializeField] private Phrase nextPhraseDefault;
    [SerializeField] private Phrase nextPhraseSuccess;
    [SerializeField] private Phrase nextPhraseFailed;

    private bool _isAvailable = true;

    public bool IsAvailable
    {
        get => _isAvailable;
        set => _isAvailable = value;
    }

    /// <summary>
    /// ��������� �������, ���� ��� ����������, � ���������� ��������� �����.
    /// </summary>
    public Phrase GetNextPhrase()
    {
        if (isCheckingChoice)
        {

            if (resultCheckAttribute && nextPhraseSuccess != null)
                return nextPhraseSuccess;
            else if (!resultCheckAttribute && nextPhraseFailed != null)
                return nextPhraseFailed;
        }
        return nextPhraseDefault;
    }

    /// <summary>
    /// ������������ �������� ��������, ������������� �� Choice � ��������� ��������� � ���������� resultCheckAttribute
    /// </summary>
    //public void ChoiceAttributeCheck()
    //{
    //    EventBus.RaiseEvent<IPlayerSubscriber>(h =>
    //    {
    //        resultCheckAttribute = h.CheckAttribute(checkAttribute, difficultCheckAttribute);

    //        if (rewardAttribute > 0)
    //            EventBus.RaiseEvent<IPlayerSubscriber>(h => h.AttributeUp(checkAttribute, rewardAttribute));

    //        Debug.Log($"��������{checkAttribute} ������ {resultCheckAttribute}");
    //    });
    //}

    /// <summary>
    /// ��������� ������� � ���������� ��������� ��������
    /// </summary>
    public bool ChoiceAttributeCheck()
    {
        if (!isCheckingChoice) return true;

        EventBus.RaiseEvent<IPlayerSubscriber>(h =>
        {
            resultCheckAttribute = h.CheckAttribute(checkAttribute, difficultCheckAttribute);

            if (rewardAttribute > 0 && resultCheckAttribute)
                h.AttributeUp(checkAttribute, rewardAttribute);
        });

        Debug.Log($"��������{checkAttribute} ������ {resultCheckAttribute}");
        return resultCheckAttribute;
    }

    public void RaiseDialogEvent()
    {
        if (isCheckingChoice)
        {
            if (resultCheckAttribute && dialogEventSuccess != null)
                dialogEventSuccess.Raise();
            else if (!resultCheckAttribute && dialogEventFailed != null)
                dialogEventFailed.Raise();
        }


        if (dialogEventDefault != null)
            dialogEventDefault.Raise();
    }

    public void ResetAvailability()
    {
        _isAvailable = true;
    }

    public string CheckAttributeText()
    {
        string nameAttribute = "";
        switch (checkAttribute)
        {
            case PlayerAttributes.Body:
                nameAttribute = "����";
                break;
            case PlayerAttributes.Mind:
                nameAttribute = "�����";
                break;
            case PlayerAttributes.Feels:
                nameAttribute = "�������";
                break;
        }
        return $"({nameAttribute} - {difficultCheckAttribute})";
    }

}
