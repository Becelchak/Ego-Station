using UnityEngine;
using EventBusSystem;

public class NoGravityZone : MonoBehaviour
{
    [SerializeField] private bool isActive = true;
    [SerializeField] private float impulseForce = 5f;
    [SerializeField] private float rotationSpeed = 30f;
    private Collider2D zoneCollider;

    private void Awake()
    {
        zoneCollider = GetComponent<Collider2D>();
        UpdateColliderState();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || !other.CompareTag("Player")) return;

        EventBus.RaiseEvent<IMoveControllerSubscriber>(s => s.OnEnterZeroGravity(this));
        EventBus.RaiseEvent<IZeroGravityController>(s => s.EnableZeroGravityMode());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            if (other.CompareTag("Dialog")) return;

            other.gameObject.layer = 8;
            other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            return;
        }

        EventBus.RaiseEvent<IMoveControllerSubscriber>(s => s.OnExitZeroGravity());
        EventBus.RaiseEvent<IZeroGravityController>(s => s.DisableZeroGravityMode());
    }

    private void UpdateColliderState()
    {
        if (zoneCollider != null)
        {
            zoneCollider.enabled = isActive;
        }
    }

    public void SetActive(bool state)
    {
        if (isActive != state)
        {
            isActive = state;
            UpdateColliderState();

            if (!isActive)
            {
                EventBus.RaiseEvent<IMoveControllerSubscriber>(s => s.OnExitZeroGravity());
            }
        }
    }
    public float GetImpulseForce() => impulseForce;
    public float GetRotationSpeed() => rotationSpeed;
}