using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/DoorHandleMiniGame")]
public class DoorHandleMiniGame : DialogEvent
{
    [Header("Settings")]
    public float maxDragDistance = 200f;
    public float maxSpeedThreshold = 50f;
    public float reboundDuration = 0.5f;
    public AnimationCurve reboundCurve;
    private DoorHandleLogic miniGameLogic;

    [Header("Feedback")]
    public DialogLogic dialog;
    public Phrase onSuccessPhrase;
    public Phrase onFailPhrase;

    public override void Raise()
    {
        miniGameLogic.Interact();
    }

    public void SetMiniGameLogic(DoorHandleLogic logic)
    {
        miniGameLogic = logic;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialog = logic;
        Debug.Log("Logic for DoorHandleMiniGame");
    }
}