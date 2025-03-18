using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ ActivateMiniGameLogicFromDialog")]
public class ActivateMiniGameLogicFromDialog : DialogEvent
{
    [SerializeField] private string logicMiniGame;
    private DialogLogic dialogLogic;
    public override void Raise()
    {
        if(GameObject.Find(logicMiniGame).TryGetComponent<IInteractive>(out var interactiveCOmponent))
        {
            interactiveCOmponent.Interact();
            return;
        }
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}
