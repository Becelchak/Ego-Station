using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeMachineDialogEvent", menuName = "Dialog Events/CoffeeMachine MiniGame Event")]
public class CoffeeMachineDialogEvent : DialogEvent
{
    private CanvasGroup miniGameCanvas;
    public override void Raise()
    {
        var coffeMashineCanvas = GameObject.Find("CoffeeMachineGame canvas");
        miniGameCanvas = coffeMashineCanvas.GetComponent<CanvasGroup>();
        miniGameCanvas.alpha = 1.0f;
        miniGameCanvas.blocksRaycasts = true;
        miniGameCanvas.interactable = true;

        coffeMashineCanvas.GetComponent<CoffeMashineGame>().UpdateUI();
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
