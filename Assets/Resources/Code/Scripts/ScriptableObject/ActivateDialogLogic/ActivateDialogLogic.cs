using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ ActivateDialogLogic")]
public class ActivateDialogLogic : DialogEvent
{
    [SerializeField] private string dialogLogicName;
    private DialogLogic dialogLogic;
    public override void Raise()
    {
        var obj = GameObject.Find($"{dialogLogicName}");
        obj.GetComponent<DialogLogic>().enabled = true;
        obj.GetComponent<BoxCollider2D>().enabled = true;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}
