using EventBusSystem;
using System;
using System.Collections;
using UnityEngine;

public class GatewayLogic : MonoBehaviour, IGateway, IGeneratorSubscriber
{
    [SerializeField] private float timeToClose = 2f;
    [SerializeField] private WireMiniGame wireMiniGame;
    private BoxCollider2D boxCollider;
    private GameObject player;
    private Animator animator;
    [SerializeField] private bool _isBlockInteract;
    [SerializeField] private string interactionText = "Взаимодействовать";
    [SerializeField] private bool isGeneratorSubscriber;
    public string InteractionText => interactionText;
    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        if (isGeneratorSubscriber) 
        {
            EventBus.Subscribe(this);
        }
    }
    public void Interact()
    {
        OnInteract?.Invoke();
        if (_isBlockInteract && wireMiniGame != null)
        {
            wireMiniGame.gameObject.SetActive(true);
            wireMiniGame.MiniGameComplete += OnMiniGameComplete;
        }
        if (_isBlockInteract) return;
        
        StartCoroutine(OpenGateway());  
    }

    private void OnMiniGameComplete(bool success)
    {
        wireMiniGame.MiniGameComplete -= OnMiniGameComplete;

        if (success)
        {
            Debug.Log("Шлюз открыт!");
            _isBlockInteract = false;
        }
        else
        {
            Debug.Log("Попробуйте еще раз.");
        }

        wireMiniGame.gameObject.SetActive(false);
        wireMiniGame = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player" || other.gameObject.name != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
        player = other.transform.gameObject;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }

    private IEnumerator OpenGateway()
    {
        animator.SetTrigger("Open");
        boxCollider.enabled = false;

        yield return new WaitForSeconds(timeToClose);

        boxCollider.enabled = true;
        animator.SetTrigger("Close");

        StopCoroutine(OpenGateway());
    }

    public void BlockInteraction()
    {
        _isBlockInteract = true;
    }

    public void OnGeneratorActivated()
    {
        _isBlockInteract = false;
    }
}
