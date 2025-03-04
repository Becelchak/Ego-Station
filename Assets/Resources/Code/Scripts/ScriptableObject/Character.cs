using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/ Character")]
public class Character : ScriptableObject
{
    [SerializeField] private string nameCharacter;
    [SerializeField] private Sprite characterImage;
    [SerializeField] private AudioClip speachSound;
    public Sprite GetSprite()
    {
        return characterImage;
    }

    public string GetName()
    {
        return nameCharacter;
    }

    public AudioClip GetSpeachSound()
    {
        return speachSound;
    }
}
