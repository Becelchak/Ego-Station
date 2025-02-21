using UnityEngine;
using static PlayerManager;

public interface IPlayerSubscriber : IGlobalSubscriber
{
    bool CheckAttribute(PlayerAttributes nameAttribute, int dificult);
}
