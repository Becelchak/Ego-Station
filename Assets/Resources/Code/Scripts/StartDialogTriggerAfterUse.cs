using NUnit.Framework;
using UnityEngine;

public class StartDialogTriggerAfterUse : MonoBehaviour
{
    [SerializeField] private DialogLogic dialogLogic;
    private IInteractive interactiveObject;
    void Start()
    {
        interactiveObject = GetComponent<IInteractive>();
        if (interactiveObject != null)
        {
            interactiveObject.OnInteract += HandleInteract;
        }
    }
    private void HandleInteract()
    {
        Debug.Log("������ ������� ����� ��������������!");
        dialogLogic.gameObject.SetActive(true);
    }
}
