using UnityEngine;

public interface IDialogEventSubscriber : IGlobalSubscriber
{
    void OnStartRiddleMiniGame(DialogLogic dialogLogic);
}
