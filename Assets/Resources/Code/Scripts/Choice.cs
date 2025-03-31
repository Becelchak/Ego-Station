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
    [SerializeField] private PlayerAttributes checkAttribute;
    public PlayerAttributes CheckAttribute => checkAttribute;
    [SerializeField] private int difficultCheckAttribute;
    [Range(0, 20)]
    [SerializeField] private double rewardAttribute = 0;
    private bool resultCheckAttribute;

    [Header("Next Phrase")]
    [SerializeField] private Phrase nextPhraseDefault;
    [SerializeField] private Phrase nextPhraseSuccess;
    [SerializeField] private Phrase nextPhraseFailed;

    /// <summary>
    /// Проверяет атрибут, если это необходимо, и возвращает следующую фразу.
    /// </summary>
    public Phrase GetNextPhrase()
    {
        if (isCheckingChoice)
        {
            ChoiceAttributeCheck();

            if (resultCheckAttribute && nextPhraseSuccess != null)
                return nextPhraseSuccess;
            else if (!resultCheckAttribute && nextPhraseFailed != null)
                return nextPhraseFailed;
        }
        return nextPhraseDefault;
    }

    /// <summary>
    /// Осуществляет проверку атрибута, закрепленного за Choice и фиксирует результат в переменной resultCheckAttribute
    /// </summary>
    public void ChoiceAttributeCheck()
    {
        EventBus.RaiseEvent<IPlayerSubscriber>(h =>
        {
            resultCheckAttribute = h.CheckAttribute(checkAttribute, difficultCheckAttribute);

            if (rewardAttribute > 0)
                EventBus.RaiseEvent<IPlayerSubscriber>(h => h.AttributeUp(checkAttribute, rewardAttribute));

            Debug.Log($"Проверка{checkAttribute} выдала {resultCheckAttribute}");
        });
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

    public string CheckAttributeText()
    {
        string nameAttribute = "";
        switch (checkAttribute)
        {
            case PlayerAttributes.Body:
                nameAttribute = "Тело";
                break;
            case PlayerAttributes.Mind:
                nameAttribute = "Разум";
                break;
            case PlayerAttributes.Feels:
                nameAttribute = "Чувства";
                break;
        }
        return $"({nameAttribute} - {difficultCheckAttribute})";
    }

}
