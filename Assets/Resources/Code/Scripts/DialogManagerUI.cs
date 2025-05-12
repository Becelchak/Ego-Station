using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerManager;

public class DialogManagerUI : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI dialogTextNoChoices;
    [SerializeField] private TextMeshProUGUI dialogTextWithChoices;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private GameObject choiseCell;
    [SerializeField] private GameObject canvasChoices;
    [SerializeField] private TextMeshProUGUI characterName;
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintText;
    [SerializeField] private float hintDisplayTime = 3f;

    private CanvasGroup dialogUICanvasGroup;
    private DialogLogic dialogLogic;
    //private Phrase _lastChoicePhrase;

    void Start()
    {
        dialogUICanvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }

    public void PrepareDialogPanel(Character character, Phrase dialogText)
    {
        dialogUICanvasGroup.alpha = 1;
        dialogUICanvasGroup.interactable = true;
        dialogUICanvasGroup.blocksRaycasts = true;

        characterImage.sprite = character.GetSprite();
        if(character.GetName() != "")
            characterName.text = $"</font=\"proxima-nova-light SDF\">[</font>{character.GetName()}</font=\"proxima-nova-light SDF\">]</font>";
        else
            characterName.text = "";
        characterName.gameObject.SetActive(true);

        var f = 0;
        while (f < choicesContainer.transform.childCount)
        {
            Destroy(choicesContainer.transform.GetChild(f).gameObject);
            f++;
        }
        if (dialogText.IsChoise)
        {
            dialogTextWithChoices.text = dialogText.TextPhrase;
            dialogTextWithChoices.gameObject.SetActive(true);
            dialogTextNoChoices.gameObject.SetActive(false);
            characterName.gameObject.SetActive(false);
            canvasChoices.gameObject.SetActive(true);

            foreach (var choice in dialogText.Choises)
            {
                var newCell = Instantiate(choiseCell);
                newCell.name = choice.name;
                newCell.SetActive(choice.IsAvailable);
                newCell.transform.SetParent(choicesContainer.transform, false);

                var cellText = newCell.GetComponentInChildren<TextMeshProUGUI>();
                string attributeText = choice.CheckAttributeText();

                if (choice.IsCheckingChoice)
                {
                    // Настраиваем цвет и стиль в зависимости от доступности
                    string styleTag = choice.IsAvailable ? "" : "<alpha=#55>";
                    string colorHex = GetAttributeColor(choice.CheckAttribute);
                    string coloredAttribute = $"<color={colorHex}>{attributeText}</color>";

                    cellText.text = $"{styleTag}{coloredAttribute} {choice.Text}";
                }
                else
                    cellText.text = $"{choice.Text}";

                var cellButton = newCell.GetComponent<Button>();
                cellButton.interactable = choice.IsAvailable;

                var localChoice = choice;
                cellButton.onClick.AddListener(() => OnChoiceSelectedUI(localChoice));
            }
        }
        else
        {
            dialogTextNoChoices.text = dialogText.TextPhrase;
            dialogTextNoChoices.gameObject.SetActive(true);
            dialogTextWithChoices.gameObject.SetActive(false);
            canvasChoices.gameObject.SetActive(false);
        }

    }
    public void ShowHint(string hint)
    {
        if (string.IsNullOrEmpty(hint)) return;

        hintText.text = hint;
        hintPanel.SetActive(true);
    }

    public void HideHint()
    {
        hintPanel.SetActive(false);
    }

    private string GetAttributeColor(PlayerAttributes attribute)
    {
        return attribute switch
        {
            PlayerAttributes.Body => "#FF0000",
            PlayerAttributes.Mind => "#ADD8E6",
            PlayerAttributes.Feels => "#800080",
            _ => "#FFFFFF"
        };
    }

    private void OnChoiceSelectedUI(Choice choice)
    {
        if (choice.IsCheckingChoice)
        {
            bool checkResult = choice.ChoiceAttributeCheck();
            choice.IsAvailable = checkResult;
            if(dialogLogic.IsAdminAccsessEnable)
                choice.IsAvailable = true;
            Debug.Log($"{choice.IsAvailable}");
        }

        // Если есть DialogEvent, вызываем его
        if (choice.DialogEventDefault != null || choice.DialogEventSuccess != null || choice.DialogEventFailed != null)
        {
            dialogLogic.SetLogicForEvent(choice);
            choice.RaiseDialogEvent();
            if(choice.DialogEventDefault == null &&  choice.DialogEventSuccess == null && choice.DialogEventFailed == null)
                dialogLogic.EndDialog();
        }

        // Если DialogEvent отсутствует, переходим к следующей фразе
        if (dialogLogic != null)
        {
            dialogLogic.OnChoiceSelected(choice);
        }

    }

    public void EndDialog()
    {
        dialogUICanvasGroup.alpha = 0;
        dialogUICanvasGroup.interactable = false;
        dialogUICanvasGroup.blocksRaycasts = false;

        dialogTextNoChoices.gameObject.SetActive(false);
        dialogTextWithChoices.gameObject.SetActive(false);
        characterName.gameObject.SetActive(false);

        var f = 0;
        while (f < choicesContainer.transform.childCount)
        {
            Destroy(choicesContainer.transform.GetChild(f).gameObject);
            f++;
        }
    }
}
