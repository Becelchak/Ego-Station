using UnityEngine;
using EventBusSystem;

[CreateAssetMenu(fileName = "HealPlayerEvent", menuName = "Dialog Events/New Heal Player Event")]
public class HealPlayerEvent : DialogEvent
{
    [SerializeField] private int healAmount = 25;

    private DialogLogic dialogLogic;

    public override void Raise()
    {
        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetHealth(healAmount));
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}