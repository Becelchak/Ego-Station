using EventBusSystem;
using System;
using UnityEngine;

public class Poster : MonoBehaviour, IInteractive
{
    [SerializeField] private SpriteRenderer posterSprite;
    [SerializeField] private GameObject hiddenObject;
    [SerializeField] private AudioClip soundInteract;
    private AudioSource AudioSource;
    private IInteractive interactiveObject;

    private bool _isBlockInteract = false;

    public event Action OnInteract;

    public bool isBlockInteract
    {
        get { return _isBlockInteract; }
        set { _isBlockInteract = value; }
    }

    private void Start()
    {
        if (hiddenObject != null)
        {
            interactiveObject = hiddenObject.GetComponent<IInteractive>();
            if (interactiveObject == null)
                Debug.LogError("Не найден компонент IInteractive!");
        }

        if (posterSprite == null)
            Debug.LogError("Спрайт плаката не назначен!");

        AudioSource = GetComponent<AudioSource>();
    }

    public void Interact()
    {
        if (_isBlockInteract)
        {
            return;
        }
        AudioSource.clip = soundInteract;
        AudioSource.Play();
        posterSprite.enabled = false;

        if (interactiveObject != null)
        {
            interactiveObject.isBlockInteract = false;
        }

        _isBlockInteract = true;
    }

    public void BlockInteraction( )
    {
        _isBlockInteract = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || _isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || _isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(this));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" || _isBlockInteract)
            return;
        EventBus.RaiseEvent<IMoveControllerSubscriber>(h => h.SetNewInteractiveObject(null));
    }
}