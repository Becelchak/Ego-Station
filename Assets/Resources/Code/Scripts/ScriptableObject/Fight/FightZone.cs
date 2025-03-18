using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ FightZone")]
public class FightZone : DialogEvent
{
    public FightLogic fight;

    // HACK: »«Ã≈Õ»“‹ ÀŒ√» ”
    public override void Raise()
    {
        var panel = GameObject.Find("Fight panel").GetComponent<CanvasGroup>();

        panel.alpha = 1.0f;
        panel.blocksRaycasts = true;
        panel.interactable = true;

        fight = GameObject.Find("Fight").GetComponent<FightLogic>();
        fight.enabled = true;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
