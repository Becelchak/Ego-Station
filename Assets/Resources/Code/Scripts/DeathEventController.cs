using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathEventController : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private GameObject fadeOverlayPrefab;
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private PlayerManager playerManager;
    private MoveController moveController;

    private void Awake()
    {
        var player = GameObject.Find("Player");
        if (player != null)
        {
            playerManager = player.GetComponent<PlayerManager>();
            moveController = player.GetComponent<MoveController>();
        }
        else
        {
            Debug.LogError("Player object not found!");
        }
    }

    public void ExecuteDeathEvent(AudioClip deathSound, float fadeDuration)
    {
        StartCoroutine(DeathSequence(deathSound, fadeDuration));
    }

    private IEnumerator DeathSequence(AudioClip deathSound, float fadeDuration)
    {
        // 1. Замораживаем игрока
        if (moveController != null)
            moveController.Freeze();

        // 2. Создаем и настраиваем экземпляр затемнения
        Image fadeInstance = null;
        if (fadeOverlayPrefab != null)
        {
            var uiCanvas = GameObject.Find("PlayerUI canvas");
            if (uiCanvas != null)
            {
                var instance = Instantiate(fadeOverlayPrefab, uiCanvas.transform);
                fadeInstance = instance.GetComponent<Image>();

                if (fadeInstance != null)
                {
                    fadeInstance.color = new Color(
                        fadeInstance.color.r,
                        fadeInstance.color.g,
                        fadeInstance.color.b,
                        0f
                    );
                }
                else
                {
                    Debug.LogError("No Image component found on fade overlay prefab!");
                }
            }
            else
            {
                Debug.LogError("UICanvas not found!");
            }
        }

        // 3. Плавное затемнение
        if (fadeInstance != null)
        {
            float elapsed = 0f;
            float startAlpha = 0f;
            float targetAlpha = 1f;
            fadeInstance.color = new Color(
                    fadeInstance.color.r,
                    fadeInstance.color.g,
                    fadeInstance.color.b,
                    fadeInstance.color.a);

            while (elapsed < fadeDuration)
            {
                float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                fadeInstance.color = new Color(
                    fadeInstance.color.r,
                    fadeInstance.color.g,
                    fadeInstance.color.b,
                    currentAlpha
                );
                Debug.Log(currentAlpha);
                Debug.Log(elapsed);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Финализируем значение
            fadeInstance.color = new Color(
                fadeInstance.color.r,
                fadeInstance.color.g,
                fadeInstance.color.b,
                targetAlpha
            );
        }

        // 4. Воспроизведение звука
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
            yield return new WaitForSeconds(deathSound.length);
        }

        // 5. Применяем смерть
        if (playerManager != null)
            playerManager.GetDamage(playerManager.Health);
    }
}