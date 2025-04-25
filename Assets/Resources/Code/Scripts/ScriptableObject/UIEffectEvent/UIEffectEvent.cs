using EventBusSystem;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "UIEffectEvent", menuName = "Dialog Events/UI Effect Event")]
public class UIEffectEvent : DialogEvent
{
    [SerializeField] private GameObject uiEffectPrefab;
    [SerializeField] private float effectDuration = 2f;
    [SerializeField] private bool destroyAfterDuration = true;
    [SerializeField] private bool isDialogLogicOneUsed;

    private GameObject currentEffectInstance;
    private DialogLogic dialogLogic;
    private Coroutine effectCoroutine;
    private float remainingTime;

    private PostUIEffectDialogActivate postUiEffectLogic;

    public override void Raise()
    {
        if (uiEffectPrefab == null)
        {
            Debug.LogError("UI Effect Prefab не назначен!");
            return;
        }

        currentEffectInstance = Instantiate(uiEffectPrefab);
        currentEffectInstance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        postUiEffectLogic = currentEffectInstance.GetComponent<PostUIEffectDialogActivate>();

        if (dialogLogic != null)
        {
            effectCoroutine = dialogLogic.StartCoroutine(HandleEffect());
            EventBus.RaiseEvent<IPlayerSubscriber>(h => h.SetNewUIEffect(this));
            if (isDialogLogicOneUsed)
                dialogLogic = null;
        }
    }

    /// <summary>
    /// Устанавливает ссылку на DialogLogic для продолжения диалога.
    /// </summary>
    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }

    /// <summary>
    /// Обрабатывает длительность эффекта и продолжает диалог после завершения.
    /// </summary>
    private IEnumerator HandleEffect()
    {
        remainingTime = effectDuration;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
            Debug.Log($"Оставшееся время эффекта: {remainingTime} секунд");
        }

        if (destroyAfterDuration && currentEffectInstance != null)
        {
            StopEffect();
        }

        if (dialogLogic != null)
        {
            dialogLogic.GoToNextPhrase();
        }

        if (postUiEffectLogic != null) 
        {
            postUiEffectLogic.RaiseLogic();
        }
    }

    /// <summary>
    /// Останавливает эффект и уничтожает его, если он активен.
    /// </summary>
    public void StopEffect()
    {
        if (currentEffectInstance != null)
        {
            Destroy(currentEffectInstance);
            if (effectCoroutine != null && dialogLogic != null)
            {
                dialogLogic.StopCoroutine(effectCoroutine);
            }
        }
    }

    /// <summary>
    /// Увеличивает длительность эффекта.
    /// </summary>
    public void AddEffectDuration(float addedTime)
    {
        effectDuration += addedTime;
        remainingTime += addedTime;
        Debug.Log($"Длительность эффекта увеличена на {addedTime} секунд. Теперь осталось: {remainingTime} секунд");
    }

    /// <summary>
    /// Возвращает оставшееся время эффекта.
    /// </summary>
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}