using UnityEngine;

[CreateAssetMenu(fileName = "CoffeMashineMiniGame", menuName = "Dialog Events/CoffeMashine MiniGame Event")]
public class CoffeMashineMiniGame : DialogEvent
{
    private CanvasGroup miniGameCanvas;
    public override void Raise()
    {
        miniGameCanvas = GameObject.Find("CoffeMashineGame canvas").GetComponent<CanvasGroup>();
        miniGameCanvas.alpha = 1.0f;
        miniGameCanvas.blocksRaycasts = true;
        miniGameCanvas.interactable = true;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
