using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialog Events/ QTEMiniGame")]
public class QTEMiniGame : DialogEvent
{
    [Header("QTE Settings")]
    [SerializeField] private float freezeTime = 2f; // Время "замедления"
    [SerializeField] private float reactionTime = 1.5f; // Время на реакцию

    [SerializeField] private QTEMiniGameLogic miniGameLogic;
    private DialogLogic dialog;



    public override void Raise()
    {

    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialog = logic;
    }
}