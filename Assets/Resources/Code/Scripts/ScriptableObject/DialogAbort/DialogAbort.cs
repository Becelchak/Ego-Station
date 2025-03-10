using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ DialogAbort")]
public class DialogAbort : DialogEvent
{
    public override void Raise()
    {
        var obj = GameObject.Find("Dialog UI canvas");
        var dialogUI = obj.GetComponent<DialogManagerUI>();
        dialogUI.EndDialog();
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
