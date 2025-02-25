using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/ DialogEvent/ FightZone")]
public class FightZone : DialogEvent
{
    public FightLogic fight;
    public override void Raise()
    {
        var panel = GameObject.Find("Fight panel").GetComponent<CanvasGroup>();

        panel.alpha = 1.0f;
        panel.blocksRaycasts = true;
        panel.interactable = true;

        fight = GameObject.Find("Fight").GetComponent<FightLogic>();
        fight.enabled = true;
    }
}
