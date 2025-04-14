using UnityEngine;

public interface IFeedbackSubscriber : IGlobalSubscriber
{
    void ShowFeedback(FeedbackData feedbackData);
    void OnFeedbackCompleted();
    void EnqueueFeedback(FeedbackData feedback);
}

[System.Serializable]
public class FeedbackData
{
    public Sprite Icon;
    public AudioClip Sound;
    public Color TextColor;
    public string Text;
    public GameObject Prefab;
    public System.Action Callback;
}
