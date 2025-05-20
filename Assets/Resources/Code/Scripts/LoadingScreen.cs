using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Text;

public class LoadingScreen : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private CanvasGroup backgroundImage;
    [SerializeField] private Image helmetImage;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float maxAlpha = 0.9f;

    private AsyncOperation loadingOperation;
    private CanvasGroup textCanvasGroup;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        textCanvasGroup = loadingText.GetComponent<CanvasGroup>();
    }

    public void ShowLoadingScreen(string sceneName)
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Начинаем анимацию
        StartCoroutine(AnimateHelmet());
        StartCoroutine(AnimateText());
        backgroundImage.interactable = true;
        backgroundImage.blocksRaycasts = true;

        while (backgroundImage.alpha < 1)
        {
            backgroundImage.alpha  += Time.deltaTime;
            yield return null;
        }

        // Загрузка сцены в фоне
        loadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadingOperation.allowSceneActivation = false;
        #if UNITY_WEBGL
            Application.backgroundLoadingPriority = ThreadPriority.Low;
        #endif

        // Ждем завершения загрузки
        while (!loadingOperation.isDone)
        {
            Debug.Log($"{loadingOperation.progress}");
            // Дожидаемся 90% загрузки
            if (loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
            }

            yield return null;
        }
        loadingPanel.SetActive(false);
        Destroy(gameObject);
        // Скрываем экран загрузки
        yield return new WaitForSeconds(1f); // Искусственная задержка для плавности
    }

    private IEnumerator AnimateHelmet()
    {
        while (loadingPanel.activeSelf)
        {
            helmetImage.transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator AnimateText()
    {
        float t = 0;
        var strBuild = new StringBuilder();
        while (loadingPanel.activeSelf)
        {
            t += pulseSpeed;
            strBuild.Clear();
            strBuild.Append(loadingText.text);
            strBuild.Append(".");
            if(t >= 3)
            {
                t = 0;
                loadingText.text = "ЗАГРУЗКА";
            }
            yield return null;
        }
    }
}