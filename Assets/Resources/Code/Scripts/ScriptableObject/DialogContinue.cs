using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/ DialogEvent/ DialogContinue")]

public class DialogContinue : DialogEvent
{
    [SerializeField] private DialogData nextDialog;
    public override void Raise()
    {
        EventBus.RaiseEvent<IDialog>(h =>
        {
            if (h.IsContinuesDialog)
                h.ChangeDialogData(nextDialog);
        });
    }
}
