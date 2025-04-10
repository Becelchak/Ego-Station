using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeMachineDialogEvent", menuName = "Dialog Events/CoffeeMachine MiniGame Event")]
public class CoffeeMachineDialogEvent : DialogEvent
{
    private CanvasGroup miniGameCanvas;
    public override void Raise()
    {
        miniGameCanvas = GameObject.Find("CoffeeMachineGame canvas").GetComponent<CanvasGroup>();
        miniGameCanvas.alpha = 1.0f;
        miniGameCanvas.blocksRaycasts = true;
        miniGameCanvas.interactable = true;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
