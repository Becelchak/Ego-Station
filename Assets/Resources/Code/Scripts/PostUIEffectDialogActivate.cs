using UnityEngine;

public class PostUIEffectDialogActivate : MonoBehaviour
{
    [SerializeField] private string dialogLogicName;
    [SerializeField] private bool needActive;
    private BoxCollider2D dialogLogicCollider;

    private void Start()
    {
        dialogLogicCollider = GameObject.Find(dialogLogicName).GetComponent<BoxCollider2D>();
    }
    public void RaiseLogic()
    {
        dialogLogicCollider.enabled = needActive;
    }
}
