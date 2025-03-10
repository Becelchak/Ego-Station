using EventBusSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class DialogManagerUI : MonoBehaviour
{
    [SerializeField] private Image characterImage;
    [SerializeField] private TextMeshProUGUI dialogTextNoChoices;
    [SerializeField] private TextMeshProUGUI dialogTextWithChoices;
    [SerializeField] private GameObject choicesContainer;
    [SerializeField] private GameObject choiseCell;
    [SerializeField] private TextMeshProUGUI characterName;

    private CanvasGroup dialogUICanvasGroup;
    private DialogLogic dialogLogic;

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
        characterName.text = character.GetName();
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
            foreach (var choice in dialogText.Choises)
            {
                var newCell = Instantiate(choiseCell);
                newCell.name = choice.name;
                newCell.SetActive(true);
                newCell.transform.SetParent(choicesContainer.transform);

                var cellText = newCell.GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = choice.Text;

                var cellButton = newCell.GetComponent<Button>();
                var localChoice = choice;
                //cellButton.onClick.AddListener(dialogLogic.GoToNextPhrase);
                ////cellButton.onClick.AddListener(localChoice.ChoiceAttributeCheck);
                //cellButton.onClick.AddListener(localChoice.RiseDialogEvent);

                // Подписываемся на событие нажатия кнопки
                cellButton.onClick.AddListener(() => OnChoiceSelectedUI(localChoice));
            }
            
        }
        else
        {
            dialogTextNoChoices.text = dialogText.TextPhrase;
            dialogTextNoChoices.gameObject.SetActive(true);
            dialogTextWithChoices.gameObject.SetActive(false);
        }

    }

    private void OnChoiceSelectedUI(Choice choice)
    {
        // Если есть DialogEvent, вызываем его
        if (choice.DialogEventDefault != null || choice.DialogEventSuccess != null || choice.DialogEventFailed != null)
        {
            dialogLogic.SetLogicForEvent(choice);
            choice.ChoiceAttributeCheck();
            choice.RaiseDialogEvent();
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
