using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ DisactivateDialogLogic")]
public class DisactivateDialogLogic : DialogEvent
{
    [SerializeField] private string dialogLogicName;
    private DialogLogic dialogLogic;
    public override void Raise()
    {
        var obj = GameObject.Find($"{dialogLogicName}");
        if (obj != null)
        {
            dialogLogic = obj.GetComponent<DialogLogic>();
        }
        dialogLogic.enabled = false;
        obj.GetComponent<BoxCollider2D>().enabled = false;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}
