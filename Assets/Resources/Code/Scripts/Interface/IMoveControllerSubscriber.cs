using UnityEngine;
using static Player.MoveController;

public interface IMoveControllerSubscriber : IGlobalSubscriber
{
    void SetNewInteractiveObject(IInteractive newInteractive);
    void Freeze();
    void Unfreeze();
    //void ChangePlayerSide(CordinateSide side);
}
