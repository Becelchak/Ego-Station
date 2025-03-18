using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Dialog Events/FindInTableMiniGame")]
public class FindInTableMiniGame : DialogEvent
{
    [SerializeField] private CanvasGroup uiTable;
    [SerializeField] private List<Phrase> onToggleMiniGamePhrase = new();
    [SerializeField] private List<GameObject> itemPrefabs;
    [SerializeField] private List<GameObject> clutterPrefabs;
    [SerializeField] private GameObject background;

    private int counter;
    private int totalItems;
    private bool isMiniGameActive = false;
    private InputAction toggleMiniGameAction; 
    private DialogLogic dialogLogic;

    public override void Raise()
    {
        InitializeMiniGame();
    }

    private void InitializeMiniGame()
    {
        counter = 0;
        totalItems = itemPrefabs.Count;

        uiTable.alpha = 1;
        uiTable.blocksRaycasts = true;
        uiTable.interactable = true;

        GenerateItems();

        toggleMiniGameAction = InputSystem.actions.FindAction("Cancel");
        if (toggleMiniGameAction != null)
        {
            toggleMiniGameAction.performed += OnToggleMiniGame;
            toggleMiniGameAction.Enable();
        }
        else
        {
            Debug.LogError("Input Action 'Cancel' не найден!");
        }

        isMiniGameActive = true;
    }

    private void GenerateItems()
    {
        // Очищаем стол от предыдущих предметов
        foreach (Transform child in uiTable.transform)
        {
            Destroy(child.gameObject);
        }

        // Добавляем задний фон
        var back = Instantiate(background, uiTable.transform);

        var rectTransform = back.transform.GetChild(0).GetComponent<RectTransform>();
        Debug.Log($"{-rectTransform.sizeDelta.x},{rectTransform.sizeDelta.x}");

        // Добавляем искомые предметы
        foreach (var itemPrefab in itemPrefabs)
        {
            var item = Instantiate(itemPrefab, uiTable.transform);
            item.GetComponent<Button>().onClick.AddListener(() => FindItem(item));

            var newX = Random.Range(-rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.x / 2);
            var newY = Random.Range(-rectTransform.sizeDelta.y / 2, rectTransform.sizeDelta.y / 2);
            item.transform.localPosition = new Vector3(newX, newY);
        }

        // Добавляем посторонние предметы
        foreach (var clutterPrefab in clutterPrefabs)
        {
            var trashItem = Instantiate(clutterPrefab, uiTable.transform);
            var newX = Random.Range(-rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.x / 2);
            var newY = Random.Range(-rectTransform.sizeDelta.y / 2, rectTransform.sizeDelta.y / 2);
            trashItem.transform.localPosition = new Vector3(newX, newY);
        }
    }

    private void OnToggleMiniGame(InputAction.CallbackContext context)
    {
        if (isMiniGameActive)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;
            isMiniGameActive = false;

            // Показываем случайную фразу
            if (dialogLogic != null && onToggleMiniGamePhrase.Count > 0)
            {
                var rndPhraseIndex = Random.Range(0, onToggleMiniGamePhrase.Count);
                dialogLogic.SetCurrentPhrase(onToggleMiniGamePhrase[rndPhraseIndex]);
            }

            Debug.Log("Мини-игра свернута. Вы можете продолжить диалог.");
        }
        else
        {
            uiTable.alpha = 1;
            uiTable.blocksRaycasts = true;
            uiTable.interactable = true;
            isMiniGameActive = true;

            Debug.Log("Мини-игра развернута. Продолжайте поиск.");
        }
    }

    public void FindItem(GameObject item)
    {
        counter++;
        Destroy(item);

        Debug.Log($"Собрано {counter} из {totalItems} предметов");

        if (counter == totalItems)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;

            OnDisable();
            Debug.Log("Вы собрали все предметы!");
        }
    }

    private void OnDisable()
    {
        if (toggleMiniGameAction != null)
        {
            toggleMiniGameAction.performed -= OnToggleMiniGame;
            toggleMiniGameAction.Disable();
        }
    }

    public override void SetDialogLogic(DialogLogic logic)
    {
        dialogLogic = logic;
    }

    public void SetUI(CanvasGroup ui)
    {
        uiTable = ui;
    }

    public void SetItems(List<GameObject> items, List<GameObject> clutter)
    {
        itemPrefabs = items;
        clutterPrefabs = clutter;
    }

    public void SetBackground(GameObject newBackground)
    {
        background = newBackground;
    }
}