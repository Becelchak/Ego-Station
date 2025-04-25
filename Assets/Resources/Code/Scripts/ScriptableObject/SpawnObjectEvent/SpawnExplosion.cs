using UnityEngine;


[CreateAssetMenu(menuName = "Dialog Events/ SpawnExplosion")]
public class SpawnExplosion : DialogEvent
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private string spawnTarget;
    private DialogLogic dialog;
    public override void Raise()
    {
        var newExplosion = Instantiate(explosion);
        var targetObj = GameObject.Find(spawnTarget);
        newExplosion.transform.position = targetObj.transform.position;
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialog = logic;
    }
}
