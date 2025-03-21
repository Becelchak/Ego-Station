using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/StartRiddleMiniGame")]
public class StartRiddleMiniGameEvent : DialogEvent
{
    [SerializeField] private string miniGameLogicObjectName;
    private DialogLogic dialogLogic;
    private RiddlesLogic riddlesLogic;

    public override void Raise()
    {
        var riddlesLogic = GameObject.Find(miniGameLogicObjectName).GetComponent<RiddlesLogic>();
        EventBus.RaiseEvent<IDialogEventSubscriber>(h => h.OnStartRiddleMiniGame(dialogLogic));
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}