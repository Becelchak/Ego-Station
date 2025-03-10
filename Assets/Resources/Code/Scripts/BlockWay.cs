using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/BlockWay")]
public class BlockWay : MonoBehaviour
{
    [SerializeField] private GameObject objectTarget;
    private IInteractive interactiveObject;

    public void BlockInteractObject()
    {
        if (objectTarget != null)
        {
            interactiveObject = objectTarget.GetComponent<IInteractive>();
            if(interactiveObject != null )
                interactiveObject.BlockInteraction();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player")
            return;
        BlockInteractObject();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }
}
