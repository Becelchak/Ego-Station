using EventBusSystem;
using UnityEngine;

public class GeneratorObject : MonoBehaviour,IGenerator
{
    private Animator animator;

    public void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    public void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }
    public void TurnOn()
    {
        animator = GetComponent<Animator>();
        animator.SetTrigger("On");
    }
}
