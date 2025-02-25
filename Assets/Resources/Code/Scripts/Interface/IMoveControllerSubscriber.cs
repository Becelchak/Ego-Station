using UnityEngine;
using static Player.MoveController;

public interface IMoveControllerSubscriber : IGlobalSubscriber
{
    void SetNewInteractiveObject(IInteractive newInteractive);
    //void ChangePlayerSide(CordinateSide side);
}
