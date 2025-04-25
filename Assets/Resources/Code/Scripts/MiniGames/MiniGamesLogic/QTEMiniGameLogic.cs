using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class QTEMiniGameLogic : MonoBehaviour
{
    [SerializeField] private float reactionTime = 1.5f;
    [SerializeField] private GameObject QTECanvas;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private GameObject fallingObjectPrefab;
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject endPosition;
    private KeyCode targetKey = KeyCode.E;
    private GameObject fallingObject;

    [Header("Events")]
    [SerializeField] private DialogEvent successEvent;
    [SerializeField] private DialogEvent failEvent;

    private bool isQTEActive = false;
    private bool playerResponded = false;

    public void Initialize()
    {
        StartCoroutine(RunQTESequence());
    }
    private IEnumerator RunQTESequence()
    {
        isQTEActive = true;
        playerResponded = false;
        var canvasGroup = QTECanvas.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        fallingObject = Instantiate(fallingObjectPrefab);

        instructionText.text = $"Æìè {targetKey}!";

        Time.timeScale = 0.3f;
        float elapsed = 0f;

        while (elapsed < reactionTime && !playerResponded)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = elapsed / reactionTime;
            fallingObject.transform.position = Vector2.Lerp(
                startPosition.transform.position,
                endPosition.transform.position,
                progress
            );

            yield return null;
        }

        Time.timeScale = 1f;
        

        if (playerResponded)
        {
            successEvent?.Raise();
            Debug.Log("QTE Success!");
            
        }
        else
        {
            failEvent?.Raise();
            Debug.Log("QTE Failed!");
        }

        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isQTEActive = false;
        EndQTE();
    }

    public void EndQTE()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isQTEActive && Input.GetKeyDown(targetKey))
        {
            playerResponded = true;
            instructionText.text = "Success!";
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.name == "Player")
        {
            Initialize();
        }
    }
}
