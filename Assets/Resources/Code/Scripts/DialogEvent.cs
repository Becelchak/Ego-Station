using UnityEngine;

public abstract class DialogEvent : ScriptableObject, IDialogEvent
{
    public abstract void Raise();
}
