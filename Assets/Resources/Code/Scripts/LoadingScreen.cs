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
        //DontDestroyOnLoad(gameObject);
        textCanvasGroup = loadingText.GetComponent<CanvasGroup>();
    }

    public void ShowLoadingScreen(string sceneName)
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Активируем экран загрузки
        StartCoroutine(AnimateHelmet());
        StartCoroutine(AnimateText());
        backgroundImage.interactable = true;
        backgroundImage.blocksRaycasts = true;

        // Плавное появление фона
        float fadeDuration = 0.5f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            backgroundImage.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        backgroundImage.alpha = 1;

        // Настройка загрузки
#if UNITY_WEBGL
            Application.backgroundLoadingPriority = ThreadPriority.Low;
#endif
        loadingOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        loadingOperation.allowSceneActivation = false;

        while (!loadingOperation.isDone)
        {
            if (loadingOperation.progress >= 0.9f)
            {
                loadingOperation.allowSceneActivation = true;
                yield return StartCoroutine(UnloadMenuScene());
            }
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        loadingPanel.SetActive(false);

        //// Финализация
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        //yield return new WaitForSeconds(0.5f); // Дополнительная задержка
        //loadingPanel.SetActive(false);
    }

    private IEnumerator UnloadMenuScene()
    {
        Scene menuScene = SceneManager.GetSceneByName("Main menu");

        if (!menuScene.IsValid())
        {
            Debug.LogWarning("MainMenu scene not found by name, trying index search");
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name.Contains("Menu"))
                {
                    menuScene = scene;
                    break;
                }
            }
        }

        if (menuScene.IsValid() && menuScene.isLoaded)
        {
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(menuScene);

            if (unloadOp == null)
            {
                gameObject.SetActive(false);
                yield return null;
            }
            else
            {
                yield return unloadOp;
            }

            yield return Resources.UnloadUnusedAssets();
        }
        else
        {
            Debug.LogWarning("Menu scene already unloaded or invalid");
        }
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