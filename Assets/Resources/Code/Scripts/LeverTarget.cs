using System.Collections.Generic;
using UnityEngine;

public class LeverTarget : MonoBehaviour
{
    [SerializeField] private List<Lever> subscribersLevers = new();
    private IInteractive interactiveComponent;

    private void Awake()
    {
        interactiveComponent = GetComponent<IInteractive>();
    }
    public void RequriedLever(Lever target)
    {
        target.BlockInteraction();
        subscribersLevers.Remove(target);

        if(subscribersLevers.Count <= 0)
        {
            interactiveComponent.isBlockInteract = false;
        }
    }
}
