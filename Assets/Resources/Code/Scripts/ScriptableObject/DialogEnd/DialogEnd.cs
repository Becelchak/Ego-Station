using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/ DialogEvent/ DialogEnd")]
public class DialogEnd : DialogEvent
{
    public override void Raise()
    {
        var obj = GameObject.Find("Dialog UI canvas");
        var dialogUI = obj.GetComponent<DialogManagerUI>();
        dialogUI.EndDialog();
        EventBus.RaiseEvent<IDialog>(h =>
        {
            if (h.IsContinuesDialog)
                h.Disable();
        });
    }
}
