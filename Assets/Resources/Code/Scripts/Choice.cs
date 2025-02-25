using EventBusSystem;
using UnityEngine;
using static PlayerManager;

[CreateAssetMenu(menuName = "Dialog/ Choice")]
public class Choice : ScriptableObject
{
    public string text;
    public bool isCheckingChoice;
    public PlayerAttributes checkAttribute;
    public int difficultCheckAttribute;

    private bool resultCheckAttribute;
    [Header("Succses Check Attribute")]
    [SerializeField] public DialogEvent dialogEventSuccses;
    [Header("Failed Check Attribute")]
    [SerializeField] public DialogEvent dialogEventFailed;

    public void ChoiceAttributeCheck()
    {
        EventBus.RaiseEvent<IPlayerSubscriber>(h =>
        {
            resultCheckAttribute = h.CheckAttribute(checkAttribute, difficultCheckAttribute);
            Debug.Log($"Проверка{checkAttribute} выдала {resultCheckAttribute}");
        });
    }

    public void RiseDialogEvent()
    {
        if(dialogEventSuccses != null && resultCheckAttribute)
            dialogEventSuccses.Raise();
        else if(dialogEventFailed != null && !resultCheckAttribute)
            dialogEventFailed.Raise();
    }
}
