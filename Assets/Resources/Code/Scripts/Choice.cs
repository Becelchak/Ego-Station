using EventBusSystem;
using UnityEngine;
using static PlayerManager;

[CreateAssetMenu(menuName = "Dialog/ Choice")]
public class Choice : ScriptableObject
{
    public string text;
    public PlayerAttributes checkAttribute;
    public int difficultCheckAttribute;
    [SerializeField] public DialogEvent dialogEvent;

    public void ChoiceAttributeCheck()
    {
        EventBus.RaiseEvent<IPlayerSubscriber>(h => Debug.Log($"Проверка{checkAttribute} выдала {h.CheckAttribute(checkAttribute, difficultCheckAttribute)}"));
    }

    public void RiseDialogEvent()
    {
        if(dialogEvent != null)
            dialogEvent.Raise();
    }
}
