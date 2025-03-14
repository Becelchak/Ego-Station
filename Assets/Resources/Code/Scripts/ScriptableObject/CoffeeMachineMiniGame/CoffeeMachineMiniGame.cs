using UnityEngine;

[CreateAssetMenu(fileName = "CoffeeMachineMiniGame", menuName = "Dialog Events/CoffeeMachine MiniGame Event")]
public class CoffeeMachineMiniGame : DialogEvent
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
