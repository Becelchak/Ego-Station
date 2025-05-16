using UnityEngine;

[CreateAssetMenu(fileName = "DeactivateNoGravityZone", menuName = "Dialog Events/DeactivateNoGravityZone")]
public class DeactivateNoGravityZone : DialogEvent
{
    [SerializeField] private string noGravityZoneName;
    private DialogLogic dialog;
    public override void Raise()
    {
        if (noGravityZoneName != null)
        {
            var noGravityZone = GameObject.Find(noGravityZoneName).GetComponent<NoGravityZone>();
            noGravityZone.SetActive(false);
        }
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialog = logic;
    }
}
