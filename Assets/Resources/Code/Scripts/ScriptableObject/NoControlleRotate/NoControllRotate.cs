using UnityEngine;
using EventBusSystem;

[CreateAssetMenu(fileName = "NoControllRotateEvent", menuName = "Dialog Events/No Control Rotate Event")]
public class NoControllRotateEvent : DialogEvent
{
    [Header("Rotation Settings")]
    [SerializeField] private float minRotationForce = 100f;
    [SerializeField] private float maxRotationForce = 300f;
    [SerializeField] private float duration = 2f;
    [SerializeField] private AudioClip rotationSound;

    private DialogLogic dialogLogic;

    public override void Raise()
    {
        EventBus.RaiseEvent<INoControllRotateSubscriber>(s => s.OnApplyRandomRotation(
            Random.Range(minRotationForce, maxRotationForce) * (Random.value > 0.5f ? 1 : -1),
            duration
        ));
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }
}

public interface INoControllRotateSubscriber : IGlobalSubscriber
{
    void OnApplyRandomRotation(float rotationForce, float duration);
}