using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UIElements;

public class CrushMiniGame : MonoBehaviour
{
    [SerializeField] private GameObject activePointPrefab;
    [SerializeField] private int pointsToComplete = 3; 
    [SerializeField] private float timeToClick = 2f;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private RawImage backgroundUI;

    private int pointsClicked = 0; 
    private bool isGameActive = false;
    private CanvasGroup canvasGroup;
    private GameObject crushedObjectPrefab;
    
    public delegate void OnMiniGameComplete(bool success);
    public event OnMiniGameComplete MiniGameComplete;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void StartMiniGame(GameObject objectPrefab, Texture backgroundPrefab)
    {
        if (isGameActive) return;
        if (objectPrefab == null)
        {
            Debug.LogError("Не назначен объект взлома!");
            return;
        }

        crushedObjectPrefab = Instantiate(objectPrefab, transform);
        crushedObjectPrefab.transform.localPosition = Vector3.zero;
        backgroundUI.texture = backgroundPrefab;
        isGameActive = true;
        pointsClicked = 0;

        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        StartCoroutine(SpawnPoints());
    }

    private IEnumerator SpawnPoints()
    {
        for (int i = 0; i < pointsToComplete; i++)
        {
            var activePoint = Instantiate(activePointPrefab, transform);
            activePoint.transform.localPosition = GetRandomPosition();
            var pointButton = activePoint.GetComponent<UnityEngine.UI.Button>();
            pointButton.onClick.AddListener(() => OnPointClicked(activePoint));

            StartCoroutine(PointTimer(activePoint));

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Vector2 GetRandomPosition()
    {
        RectTransform rectTransform = crushedObjectPrefab.GetComponent<RectTransform>();
        var x = Random.Range(-rectTransform.sizeDelta.x * 10, rectTransform.sizeDelta.x * 10);
        var y = Random.Range(-rectTransform.sizeDelta.y * 10, rectTransform.sizeDelta.y * 10);
        return new Vector2(x, y);
    }

    private IEnumerator PointTimer(GameObject activePoint)
    {
        UnityEngine.UI.Image circle = activePoint.GetComponent<UnityEngine.UI.Image>();
        var timer = timeToClick;

        while (timer > 0 && activePoint != null)
        {
            circle.fillAmount = timer / timeToClick;
            timer -= Time.deltaTime;
            yield return null;
        }

        if (activePoint != null)
        {
            Destroy(activePoint);
            MiniGameComplete?.Invoke(false);
            isGameActive = false;
            EndGame();
        }
    }

    public void OnPointClicked(GameObject activePoint)
    {
        pointsClicked++;
        Destroy(activePoint);

        if (pointsClicked >= pointsToComplete)
        {
            MiniGameComplete?.Invoke(true);
            isGameActive = false;
            EndGame();
        }
    }

    public void EndGame()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
        StopAllCoroutines();
    }
}