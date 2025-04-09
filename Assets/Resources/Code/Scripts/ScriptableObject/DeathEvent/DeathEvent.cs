using UnityEngine;

[CreateAssetMenu(fileName = "NewDeathEvent", menuName = "Dialog Events/DeathEvent")]
public class DeathEvent : DialogEvent
{
    [Header("Death Settings")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private string nameObjectWithDeathController;

    public override void Raise()
    {
        var obj = GameObject.Find(nameObjectWithDeathController);
        var deathController = obj.GetComponent<DeathEventController>();
        if (deathController == null)
        {
            Debug.LogError("DeathEventController not found in scene!");
            return;
        }

        deathController.ExecuteDeathEvent(deathSound, fadeDuration);
    }

    public override void SetDialogLogic(DialogLogic logic) { }
}