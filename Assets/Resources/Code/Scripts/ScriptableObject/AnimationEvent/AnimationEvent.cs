using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Events/ AnimationEvent")]
public class AnimationEvent : DialogEvent
{
    [SerializeField] AnimationClip clip;
    private Animator animatorPlayer;
    public override void Raise()
    {
        animatorPlayer = GameObject.Find("Player").GetComponent<Animator>();
        animatorPlayer.Play(clip.name);
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}
