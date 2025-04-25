using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/TeleportPlayer")]
public class TeleportPlayer : DialogEvent
{
    [SerializeField] private string pointTeleportPlayerName;
    private Transform player;
    private DialogLogic dialogLogic;
    public override void Raise()
    {
        var point = GameObject.Find(pointTeleportPlayerName);
        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.TeleportPlayer(point.transform));
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}
