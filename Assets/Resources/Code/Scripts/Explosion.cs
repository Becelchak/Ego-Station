using EventBusSystem;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int damage;
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (IsAnimationPlaying())
        {
        }
        else
        {
            Destroy(gameObject);
        }
    }

    bool IsAnimationPlaying()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime < 1.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(damage));
        }
    }
}
