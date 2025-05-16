using UnityEngine;

[CreateAssetMenu(fileName = "GiveItem", menuName = "Dialog Events/GiveItem")]
public class GiveItem : DialogEvent
{
    [SerializeField] private string rewardItemId;
    [SerializeField] private int rewardQuantity;
    private DialogLogic dialog;
    public override void Raise()
    {
        GameManager.Instance.CollectItem(rewardItemId, rewardQuantity);
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialog = logic;
    }
}
