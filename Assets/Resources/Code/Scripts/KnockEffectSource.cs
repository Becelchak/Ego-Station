using EventBusSystem;
using UnityEngine;

public class KnockEffectSource : MonoBehaviour
{
    [SerializeField] private DialogEvent uiEffect;
    [SerializeField] private string dialogName;

    private void ActivateEffect()
    {
        gameObject.layer = 8;
        var dialog = GameObject.Find(dialogName);

        if (dialog != null && uiEffect != null)
        {
            uiEffect.SetDialogLogic(dialog.GetComponent<DialogLogic>());
            uiEffect.Raise();
        }

        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(80));
        EventBus.RaiseEvent<IQTEFinishSubscriber>(h => h.OnQTEFinished(false));

        if (TryGetComponent<AudioSource>(out var audioSource))
        {
            audioSource.Play();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.name == "Player")
        {
            ActivateEffect();
        }
    }
}