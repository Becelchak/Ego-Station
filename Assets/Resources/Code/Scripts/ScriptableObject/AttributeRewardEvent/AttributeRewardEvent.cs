using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ AttributeRewardEvent")]
public class AttributeRewardEvent : DialogEvent
{
    [SerializeField] private PlayerManager.PlayerAttributes attribute = PlayerManager.PlayerAttributes.Body;
    [SerializeField] private double upPoint = 0;
    private PlayerManager playerManager;
    public override void Raise()
    {
        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.AttributeUp(attribute, upPoint));
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
