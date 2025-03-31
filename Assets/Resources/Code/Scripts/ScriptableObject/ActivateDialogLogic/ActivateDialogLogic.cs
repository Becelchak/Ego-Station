using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ ActivateDialogLogic")]
public class ActivateDialogLogic : DialogEvent
{
    [SerializeField] private string dialogLogic;
    public override void Raise()
    {
        var obj = GameObject.Find($"{dialogLogic}");
        obj.GetComponent<DialogLogic>().enabled = true;
        obj.GetComponent<BoxCollider2D>().enabled = true;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
    }

}
