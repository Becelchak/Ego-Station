using UnityEngine;
using static Player.MoveController;

public interface IMoveSubscriber : IGlobalSubscriber
{
    void Test();
    void SetNewInteractiveObject(IInteractive newInteractive);
    void ChangePlayerSide(CordinateSide side);
}
