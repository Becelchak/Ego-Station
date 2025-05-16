using System;
using UnityEngine;

public class WireMiniGameSubActivate : MonoBehaviour, IInteractive
{
    [SerializeField] private string interactionText = "Шаманить с проводами";
    [Header("MiniGame parameteres")]
    [SerializeField] private WireMiniGame wireGame;
    [SerializeField] private int wireCount;
    [SerializeField] private float wireDistance;
    public string InteractionText => interactionText;
    [Header("Dialogs")]
    [SerializeField] private DialogLogic dialogAfterSuccess;
    [SerializeField] private DialogLogic dialogAfterFailed;

    private bool _isBlockInteract;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    public void BlockInteraction()
    {
        _isBlockInteract = false;
    }

    public void Interact()
    {
        wireGame.ChangeParameteres(wireCount, wireDistance);
        wireGame.gameObject.SetActive(true);
        wireGame.MiniGameComplete += OnMiniGameComplete;
    }

    private void OnMiniGameComplete(bool success)
    {
        wireGame.MiniGameComplete -= OnMiniGameComplete;

        if (success)
        {
            if (dialogAfterSuccess != null)
                dialogAfterSuccess.gameObject.SetActive(true);
        }
        else
        {
            if (dialogAfterFailed != null)
                dialogAfterFailed.gameObject.SetActive(true);
        }

        wireGame.gameObject.SetActive(false);
        wireGame = null;
    }
}
