using System;
using System.Collections;
using System.Collections.Generic;
using EventBusSystem;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour, IPlayerSubscriber, IFeedbackSubscriber
{
    [System.Serializable]
    public class AttributeFeedback
    {
        public Sprite icon;
        public AudioClip sound;
        public Color textColor = Color.white;
    }

    [System.Serializable]
    public class AttributeIcons
    {
        public AttributeFeedback bodySuccess;
        public AttributeFeedback bodyFail;
        public AttributeFeedback mindSuccess;
        public AttributeFeedback mindFail;
        public AttributeFeedback feelsSuccess;
        public AttributeFeedback feelsFail;
        public AttributeFeedback attributeUpBody;
        public AttributeFeedback attributeUpMind;
        public AttributeFeedback attributeUpFeels;
    }

    [Header("Feedback Settings")]
    [SerializeField] private AttributeIcons attributeFeedback;
    [SerializeField] private GameObject iconDefaultPrefab;
    [SerializeField] private GameObject iconPrefabUp;
    [SerializeField] private GameObject iconPrefabCheck;
    [SerializeField] private Transform iconSpawnPoint;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayTime = 1.5f;
    [SerializeField] private AudioSource feedbackAudioSource;
    private Queue<FeedbackData> feedbackQueue = new Queue<FeedbackData>();
    private bool isProcessingFeedback = false;
    private FeedbackData currentFeedback;

    [Header("Attribute")]
    [SerializeField] private double bodyAttribute = 1;
    [SerializeField] private double mindAttribute = 1;
    [SerializeField] private double feelsAttribute = 1;
    public double BodyAttribute
    {
        get { return bodyAttribute; }
        set { bodyAttribute = Math.Min(20, value); }
    }

    public double MindAttribute
    {
        get { return mindAttribute; }
        set { mindAttribute = Math.Min(20, value); }
    }

    public double FeelsAttribute
    {
        get { return feelsAttribute; }
        set { feelsAttribute = Math.Min(20, value); }
    }
    [Header("OtherParameters")]
    [SerializeField] private int health = 100;
    [SerializeField] private AudioClip damageAudio;
    private Coroutine rotationCoroutine;
    private AudioSource otherAudioSource;

    public int Health
    {
        get { return health; }
        private set
        {
            health = Math.Max(0, value);
            if (health <= 0)
            {
                Die();
            }
            else if (health > 100) 
            {
                health = 100;
            }
        }
    }

    private UIEffectEvent nowEffect;
    private Animator animator;
    private MoveController moveController;

    [Header("Death UI")]
    [SerializeField] private GameObject deathUIPrefab;
    [SerializeField] private GameObject playerUI;
    [Header("Player Manager UI")]
    [SerializeField] private GameObject playerManagerCanvas;
    private Image healbarImage;
    private TextMeshProUGUI bodyTextUI;
    private TextMeshProUGUI mindTextUI;
    private TextMeshProUGUI feelsTextUI;

    private void OnEnable()
    {
        EventBus.Subscribe(this);
        moveController = GetComponent<MoveController>();
        animator = GetComponent<Animator>();
        otherAudioSource = gameObject.transform.GetChild(0).GetComponent<AudioSource>();
        healbarImage = playerManagerCanvas.transform.GetChild(1).GetChild(1).GetComponent<Image>();
        bodyTextUI = playerManagerCanvas.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();
        mindTextUI = playerManagerCanvas.transform.GetChild(3).GetComponentInChildren<TextMeshProUGUI>();
        feelsTextUI = playerManagerCanvas.transform.GetChild(4).GetComponentInChildren<TextMeshProUGUI>();

        bodyTextUI.text = $"{BodyAttribute}";
        mindTextUI.text = $"{MindAttribute}";
        feelsTextUI.text = $"{FeelsAttribute}";
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(this);
    }

    public bool CheckAttribute(PlayerAttributes attribute, int difficulty)
    {
        AttributeFeedback feedback = GetCheckResultFeedback(attribute, difficulty, out bool result);

        if (feedback != null)
        {
            EventBus.RaiseEvent<IFeedbackSubscriber>(s => s.EnqueueFeedback(
                new FeedbackData
                {
                    Icon = feedback.icon,
                    Sound = feedback.sound,
                    TextColor = feedback.textColor,
                    Text = "",
                    Prefab = iconPrefabCheck,
                    Callback = () => {
                        UpdateAttributeUI();
                    }
                }
            ));
        }

        return result;
    }

    public void GetDamage(int damagePoints)
    {
        animator.Play("GetDamage");
        Health -= damagePoints;
        healbarImage.fillAmount = Mathf.Clamp01(Health / 100f);

        otherAudioSource.Stop();
        otherAudioSource.clip = damageAudio;
        otherAudioSource.Play();
        Debug.Log($"{health}");
    }

    public void GetHealth(int healthPoints)
    {
        Health += healthPoints;
        healbarImage.fillAmount = Mathf.Clamp01(Health / 100f);

        Debug.Log($"{health}");
    }

    public void AttributeUp(PlayerAttributes attribute, double rewardCount)
    {
        if (rewardCount <= 0) return;

        AttributeFeedback feedback = GetAttributeUpFeedback(attribute);
        string text = $"[      +{rewardCount}]";

        switch (attribute)
        {
            case PlayerAttributes.Body:
                bodyAttribute = Math.Min(20, bodyAttribute + rewardCount);
                break;
            case PlayerAttributes.Mind:
                mindAttribute = Math.Min(20, mindAttribute + rewardCount);
                break;
            case PlayerAttributes.Feels:
                feelsAttribute = Math.Min(20, feelsAttribute + rewardCount);
                break;
        }

        EventBus.RaiseEvent<IFeedbackSubscriber>(s => s.EnqueueFeedback(
            new FeedbackData
            {
                Icon = feedback.icon,
                Sound = feedback.sound,
                TextColor = feedback.textColor,
                Text = text,
                Prefab = iconPrefabUp,
                Callback = UpdateAttributeUI
            }
        ));
    }

    private void UpdateAttributeUI()
    {
        bodyTextUI.text = $"{BodyAttribute}";
        mindTextUI.text = $"{MindAttribute}";
        feelsTextUI.text = $"{FeelsAttribute}";
    }

    public void SetNewUIEffect(UIEffectEvent newEffect)
    {
        if(newEffect != null)
            nowEffect = newEffect;
    }

    public void RemoveUIEffect()
    {
        nowEffect.StopEffect();
        nowEffect = null;
    }

    public void UpdateUIEffect(float time)
    {
        Debug.Log($"{nowEffect}");
        if(nowEffect != null)
            nowEffect.AddEffectDuration(time);
    }

    private void Die()
    {
        Debug.Log("Death");
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        if (moveController != null)
        {
            moveController.Freeze();
        }

        if (deathUIPrefab != null)
        {
            var newDeathPanel = Instantiate(deathUIPrefab);
            newDeathPanel.transform.SetParent(playerUI.transform, false);
        }
        else
        {
            Debug.LogWarning("Death UI Prefab is not assigned in PlayerManager.");
        }

        EventBus.RaiseEvent<IPlayerDeathSubscriber>(s => s.OnPlayerDeath());
        // Дополнительные действия при смерти (например, остановка времени)
        Time.timeScale = 0f;
    }

    public void ShowFeedback(FeedbackData feedbackData)
    {
        StartCoroutine(ShowSingleFeedback(feedbackData));
    }

    public void OnFeedbackCompleted()
    {
        try
        {
            var callback = currentFeedback?.Callback;
            currentFeedback = null;
            isProcessingFeedback = false;

            callback?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"OnFeedbackCompleted error: {e.Message}");
        }
    }

    private AttributeFeedback GetCheckResultFeedback(PlayerAttributes attribute, int difficulty, out bool result)
    {
        int minPoint = GetMinAttributeValue(attribute);
        result = UnityEngine.Random.Range(minPoint, 20) >= difficulty;

        return attribute switch
        {
            PlayerAttributes.Body => result ? attributeFeedback.bodySuccess : attributeFeedback.bodyFail,
            PlayerAttributes.Mind => result ? attributeFeedback.mindSuccess : attributeFeedback.mindFail,
            PlayerAttributes.Feels => result ? attributeFeedback.feelsSuccess : attributeFeedback.feelsFail,
            _ => null
        };
    }

    private AttributeFeedback GetAttributeUpFeedback(PlayerAttributes attribute)
    {
        return attribute switch
        {
            PlayerAttributes.Body => attributeFeedback.attributeUpBody,
            PlayerAttributes.Mind => attributeFeedback.attributeUpMind,
            PlayerAttributes.Feels => attributeFeedback.attributeUpFeels,
            _ => null
        };
    }

    private int GetMinAttributeValue(PlayerAttributes attribute)
    {
        double value = attribute switch
        {
            PlayerAttributes.Body => bodyAttribute,
            PlayerAttributes.Mind => mindAttribute,
            PlayerAttributes.Feels => feelsAttribute,
            _ => 1
        };
        return (int)Math.Ceiling(Math.Max(1, value));
    }

    private IEnumerator ProcessFeedbackQueue()
    {
        isProcessingFeedback = true;

        while (feedbackQueue.Count > 0)
        {
            FeedbackData current = feedbackQueue.Dequeue();
            yield return StartCoroutine(ShowSingleFeedback(current));

            yield return new WaitForSeconds(fadeDuration + 0.2f);
        }

        isProcessingFeedback = false;
    }

    private IEnumerator ShowSingleFeedback(FeedbackData feedback)
    {
        var iconInstance = Instantiate(feedback.Prefab, iconSpawnPoint);
        var iconImage = iconInstance.GetComponentInChildren<Image>();
        var canvasGroup = iconInstance.GetComponent<CanvasGroup>();
        var textComponent = iconInstance.GetComponentInChildren<TextMeshProUGUI>();
        feedbackAudioSource = iconInstance.GetComponentInChildren<AudioSource>();

        iconImage.sprite = feedback.Icon;
        if (textComponent != null)
        {
            textComponent.text = feedback.Text;
            textComponent.color = feedback.TextColor;
        }

        if (feedbackAudioSource != null && feedback.Sound != null)
        {
            feedbackAudioSource.PlayOneShot(feedback.Sound);
        }

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (iconInstance == null) yield break;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            if (iconInstance == null) yield break;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        feedback.Callback?.Invoke();

        if (iconInstance != null)
        {
            Destroy(iconInstance);
        }

        if (this != null)
        {
            EventBus.RaiseEvent<IFeedbackSubscriber>(s =>
            {
                if (s != null && s is PlayerManager manager && manager == this)
                {
                    manager.OnFeedbackCompleted();
                }
            });
        }

        yield break;
    }

    void IFeedbackSubscriber.EnqueueFeedback(FeedbackData feedback)
    {
        feedbackQueue.Enqueue(feedback);

        if (!isProcessingFeedback)
        {
            StartCoroutine(ProcessFeedbackQueue());
        }
    }

    public void TeleportPlayer(Transform pointTeleport)
    {
        transform.position = pointTeleport.position;
    }
    public enum PlayerAttributes
    {
        Body,
        Mind,
        Feels
    }
}
