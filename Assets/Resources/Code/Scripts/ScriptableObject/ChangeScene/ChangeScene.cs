using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Dialog Events/ ChangeScene")]
public class ChangeScene : DialogEvent
{
    [SerializeField] private string sceneName;
    private DialogLogic dialog;
    public override void Raise()
    {
        SceneManager.LoadScene(sceneName);
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialog = logic;
    }
}
