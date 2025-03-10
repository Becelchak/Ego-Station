using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "UIEffectEvent", menuName = "Dialog Events/UI Effect Event")]
public class UIEffectEvent : DialogEvent
{
    [SerializeField] private GameObject uiEffectPrefab;
    [SerializeField] private float effectDuration = 2f;
    [SerializeField] private bool destroyAfterDuration = true;

    private GameObject currentEffectInstance;
    private DialogLogic dialogLogic;

    public override void Raise()
    {
        Debug.Log($"{dialogLogic}");

        if (uiEffectPrefab == null)
        {
            Debug.LogError("UI Effect Prefab не назначен!");
            return;
        }

        currentEffectInstance = Instantiate(uiEffectPrefab);
        currentEffectInstance.transform.SetParent(FindObjectOfType<Canvas>().transform, false);

        // Запускаем корутину для управления длительностью эффекта
        if (dialogLogic != null)
        {
            dialogLogic.StartCoroutine(HandleEffect());
        }
        else
        {
            Debug.LogError("DialogLogic не назначен!");
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
        // Ждем указанное время
        yield return new WaitForSeconds(effectDuration);

        // Уничтожаем эффект, если нужно
        if (destroyAfterDuration && currentEffectInstance != null)
        {
            StopEffect();         
        }

        // Продолжаем диалог
        if (dialogLogic != null)
        {
            dialogLogic.GoToNextPhrase();
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
            dialogLogic.StopCoroutine(HandleEffect());
        }
    }
}