using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;

public class InteractionLabelController : MonoBehaviour
{
    [SerializeField] private GameObject labelPrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float yOffset = 1.5f;
    [SerializeField] private float fadeDuration = 0.3f;

    private GameObject currentLabel;
    private TextMeshProUGUI labelText;
    private CanvasGroup canvasGroup;
    private IInteractive currentTarget;
    private float fadeTimer;
    private bool isFadingIn;
    private bool isFadingOut;

    private void Awake()
    {
        var playerUiCanvas = GameObject.Find("PlayerUI canvas");
        currentLabel = Instantiate(labelPrefab, playerUiCanvas.transform);
        currentLabel.transform.SetSiblingIndex(0);
        labelText = currentLabel.GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = currentLabel.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = currentLabel.AddComponent<CanvasGroup>();

        currentLabel.SetActive(false);
        canvasGroup.alpha = 0;
    }

    public void ShowLabel(IInteractive interactive, Vector3 worldPosition)
    {

        if (interactive == null || (interactive is MonoBehaviour mb && mb == null))
        {
            HideLabel();
            return;
        }

        Debug.Log($"{interactive.isBlockInteract}");
        if (interactive.isBlockInteract) return;

        if (currentTarget == interactive && currentLabel.activeSelf)
        {
            UpdatePosition(worldPosition);
            return;
        }

        currentTarget = interactive;
        labelText.text = $"</font=\"proxima-nova-light SDF\">[</font>E</font=\"proxima-nova-light SDF\">]</font>{interactive.InteractionText}";
        UpdatePosition(worldPosition);

        if (!isFadingIn)
        {
            fadeTimer = 0;
            isFadingIn = true;
            isFadingOut = false;
            currentLabel.SetActive(true);
        }
    }

    public void HideLabel()
    {
        // ���� ��� ���� ��� ��� ���� fadeOut, ������ �� ������
        if (currentTarget == null || isFadingOut) return;

        isFadingIn = false;
        isFadingOut = true;
        fadeTimer = 0;
    }

    private void Update()
    {
        if (currentTarget != null && currentTarget is MonoBehaviour mb && mb == null)
        {
            currentTarget = null;
            HideLabel();
            return;
        }

        // ��������� ��������
        if (isFadingIn)
        {
            fadeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(fadeTimer / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(0, 1, progress);

            if (progress >= 1f)
            {
                isFadingIn = false;
                canvasGroup.alpha = 1;
            }
        }
        else if (isFadingOut)
        {
            fadeTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(fadeTimer / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(1, 0, progress);

            if (progress >= 1f)
            {
                isFadingOut = false;
                canvasGroup.alpha = 0;
                currentLabel.SetActive(false);
                currentTarget = null;
            }
        }

        // ���������� �������
        if (currentTarget != null && canvasGroup.alpha > 0)
        {
            Vector3 position = (currentTarget is MonoBehaviour behaviour) ?
                (behaviour != null ? behaviour.transform.position : Vector3.zero) :
                Vector3.zero;
            UpdatePosition(position);
        }
    }

    private void UpdatePosition(Vector3 worldPosition)
    {
        if (!currentLabel.activeSelf) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition + Vector3.up * yOffset);
        currentLabel.transform.position = screenPos;
    }
}