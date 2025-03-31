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

        //TO DO: Убрать код текста при выборах
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

                string attributeText = choice.CheckAttributeText();

                string colorHex = "#FFFFFF";
                switch (choice.CheckAttribute)
                {
                    case PlayerAttributes.Body:
                        colorHex = "#FF0000";
                        break;
                    case PlayerAttributes.Mind:
                        colorHex = "#ADD8E6";
                        break;
                    case PlayerAttributes.Feels:
                        colorHex = "#800080"; 
                        break;
                }

                string coloredAttribute = $"<color={colorHex}>{attributeText}</color>";
                cellText.text = $"{coloredAttribute} {choice.Text}";

                var cellButton = newCell.GetComponent<Button>();
                var localChoice = choice;

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
            //choice.ChoiceAttributeCheck();
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
