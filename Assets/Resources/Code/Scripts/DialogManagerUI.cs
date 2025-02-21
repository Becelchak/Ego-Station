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
    void Start()
    {
        dialogUICanvasGroup = GetComponent<CanvasGroup>();
    }

    public void PrepareDialogPanel(Character character, Phrase dialogText)
    {
        dialogUICanvasGroup.alpha = 1;
        dialogUICanvasGroup.interactable = true;
        dialogUICanvasGroup.blocksRaycasts = true;


        characterImage.sprite = character.GetSprite();
        characterName.text = character.GetName();
        characterName.gameObject.SetActive(true);

        if (dialogText.isChoise)
        {
            var f = 0;
            while (f < choicesContainer.transform.childCount)
            {
                Destroy(choicesContainer.transform.GetChild(f).gameObject);
                f++;
            }

            dialogTextWithChoices.text = dialogText.textPhrase;
            dialogTextWithChoices.gameObject.SetActive(true);
            dialogTextNoChoices.gameObject.SetActive(false);
            foreach (var choice in dialogText.choises)
            {
                var newCell = Instantiate(choiseCell);
                newCell.name = choice.name;
                newCell.SetActive(true);
                newCell.transform.SetParent(choicesContainer.transform);

                var cellText = newCell.GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = choice.text;

                var cellButton = newCell.GetComponent<Button>();
                var localChoice = choice;
                cellButton.onClick.AddListener(localChoice.ChoiceAttributeCheck);
                cellButton.onClick.AddListener(localChoice.RiseDialogEvent);
            }
            
        }
        else
        {
            dialogTextNoChoices.text = dialogText.textPhrase;
            dialogTextNoChoices.gameObject.SetActive(true);
            dialogTextWithChoices.gameObject.SetActive(false);
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
