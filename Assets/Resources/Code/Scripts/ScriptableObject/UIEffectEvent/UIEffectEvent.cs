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
            Debug.LogError("UI Effect Prefab �� ��������!");
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
    /// ������������� ������ �� DialogLogic ��� ����������� �������.
    /// </summary>
    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }

    /// <summary>
    /// ������������ ������������ ������� � ���������� ������ ����� ����������.
    /// </summary>
    private IEnumerator HandleEffect()
    {
        remainingTime = effectDuration;

        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
            Debug.Log($"���������� ����� �������: {remainingTime} ������");
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
    /// ������������� ������ � ���������� ���, ���� �� �������.
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
    /// ����������� ������������ �������.
    /// </summary>
    public void AddEffectDuration(float addedTime)
    {
        effectDuration += addedTime;
        remainingTime += addedTime;
        Debug.Log($"������������ ������� ��������� �� {addedTime} ������. ������ ��������: {remainingTime} ������");
    }

    /// <summary>
    /// ���������� ���������� ����� �������.
    /// </summary>
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}