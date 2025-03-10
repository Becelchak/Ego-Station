using UnityEngine;
using static PlayerManager;

public interface IPlayerSubscriber : IGlobalSubscriber
{
    bool CheckAttribute(PlayerAttributes nameAttribute, int dificult);
    void AttributeUp(PlayerAttributes nameAttribute, double rewardCount);
    void GetDamage(int damagePoints);

    void SetNewUIEffect(UIEffectEvent newEffect);
    void RemoveUIEffect();
}
