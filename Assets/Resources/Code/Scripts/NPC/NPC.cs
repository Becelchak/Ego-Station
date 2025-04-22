using EventBusSystem;
using UnityEngine;

public class NPC : MonoBehaviour, INPCSubscriber
{
    [SerializeField] private double _health;
    public double health
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                isDead = true;
            }
        }
    }
    private Animator animator;

    public bool isDead;
    public bool isFighting { get;  set; }

    void Awake()
    {
        EventBus.Subscribe(this);
        animator = GetComponent<Animator>();
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe(this);
    }

    public void Attack(int damage)
    {
        animator.Play("Attack");
        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(damage));
    }

    public void GetDamage(int damage) 
    {
        health -= damage;
    }
}
