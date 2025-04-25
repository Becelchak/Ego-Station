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
        uiEffect.SetDialogLogic(dialog.GetComponent<DialogLogic>());
        uiEffect.Raise();
        EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(80));
        GetComponent<AudioSource>().Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.name == "Player")
            ActivateEffect();
    }
}
