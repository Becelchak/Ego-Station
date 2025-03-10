using System;
using EventBusSystem;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IPlayerSubscriber
{
    [Header("Attribute")]
    [SerializeField] private double bodyAttribute = 1;
    [SerializeField] private double mindAttribute = 1;
    [SerializeField] private double feelsAttribute = 1;
    public double BodyAttribute
    {
        get { return bodyAttribute; }
        set { bodyAttribute = Math.Min(20, value); }
    }

    public double MindAttribute
    {
        get { return mindAttribute; }
        set { mindAttribute = Math.Min(20, value); }
    }

    public double FeelsAttribute
    {
        get { return feelsAttribute; }
        set { feelsAttribute = Math.Min(20, value); }
    }
    [Header("OtherParameters")]
    [SerializeField] private int health = 100;

    public int Health
    {
        get { return health; }
        private set { health = Math.Max(0, value); }
    }

    private UIEffectEvent nowEffect;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }
    public bool CheckAttribute(PlayerAttributes nameAttribute, int dificult)
    {
        var minPoint = 1;
        switch (nameAttribute)
        {
            case PlayerAttributes.Body:
                minPoint = (int)Math.Ceiling(Math.Max(minPoint, bodyAttribute));
                break;
            case PlayerAttributes.Mind:
                minPoint = (int)Math.Ceiling(Math.Max(minPoint, mindAttribute));
                break;
            case PlayerAttributes.Feels:
                minPoint = (int)Math.Ceiling(Math.Max(minPoint, feelsAttribute));
                break;
            default:
                break;
        }
        var rndNumber = UnityEngine.Random.Range(minPoint, 20);
        Debug.Log($"{rndNumber}");
        return rndNumber >= dificult;
    }

    public void GetDamage(int damagePoints)
    {
        Health -= damagePoints;
    }

    public void AttributeUp(PlayerAttributes nameAttribute, double rewardCount)
    {
        switch (nameAttribute)
        {
            case PlayerAttributes.Body:
                bodyAttribute += rewardCount;
                break;
            case PlayerAttributes.Mind:
                mindAttribute += rewardCount;
                break;
            case PlayerAttributes.Feels:
                feelsAttribute += rewardCount;
                break;
            default:
                break;
        }
    }

    public void SetNewUIEffect(UIEffectEvent newEffect)
    {
        if(newEffect != null)
            nowEffect = newEffect;
    }

    public void RemoveUIEffect()
    {
        nowEffect.StopEffect();
        nowEffect = null;
    }

    public enum PlayerAttributes
    {
        Body,
        Mind,
        Feels
    }
}
