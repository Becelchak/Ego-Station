using UnityEngine;

public abstract class DialogEvent : ScriptableObject, IDialogEvent
{
    public abstract void Raise();
    public abstract void SetDialogLogic(DialogLogic logic);
}
