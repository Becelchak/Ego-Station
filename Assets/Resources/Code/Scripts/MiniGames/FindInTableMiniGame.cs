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

    [SerializeField] private string rewardItemId; // ID предмета, который получит игрок
    [SerializeField] private int rewardQuantity = 1; // Количество предметов

    private int counter;
    private int totalItems;
    private bool isMiniGameActive = false;
    [SerializeField] private bool isMiniGameInitialized = false;
    private bool isPlayerInZone = false; // Флаг нахождения игрока в зоне
    private bool isCompleted = false; // Флаг завершения мини-игры
    private InputAction toggleMiniGameAction;
    private DialogLogic dialogLogic;

    // Состояние мини-игры
    private List<Vector3> itemPositions = new(); // Позиции искомых предметов
    private List<Vector3> clutterPositions = new(); // Позиции посторонних предметов
    private List<GameObject> spawnedItems = new(); // Созданные искомые предметы
    private List<GameObject> spawnedClutter = new(); // Созданные посторонние предметы

    public override void Raise()
    {
        if (!isCompleted) // Проверяем, не завершена ли мини-игра
        {
            InitializeMiniGame();
        }
        else
        {
            Debug.Log("Мини-игра уже завершена. Взаимодействие заблокировано.");
        }
    }

    public void OnEnable()
    {
        isCompleted = false;
    }

    private void InitializeMiniGame()
    {
        if (isMiniGameInitialized)
        {
            RestoreMiniGame();
        }
        else
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
            isMiniGameInitialized = true;
        }
    }

    private void GenerateItems()
    {
        foreach (Transform child in uiTable.transform)
        {
            Destroy(child.gameObject);
        }

        var back = Instantiate(background, uiTable.transform);
        var rectTransform = back.transform.GetChild(0).GetComponent<RectTransform>();

        foreach (var itemPrefab in itemPrefabs)
        {
            var item = Instantiate(itemPrefab, uiTable.transform);
            item.GetComponent<Button>().onClick.AddListener(() => FindItem(item));

            var newX = Random.Range(-rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.x / 2);
            var newY = Random.Range(-rectTransform.sizeDelta.y / 2, rectTransform.sizeDelta.y / 2);
            item.transform.localPosition = new Vector3(newX, newY);

            spawnedItems.Add(item);
            itemPositions.Add(item.transform.localPosition);
        }

        foreach (var clutterPrefab in clutterPrefabs)
        {
            var trashItem = Instantiate(clutterPrefab, uiTable.transform);
            var newX = Random.Range(-rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.x / 2);
            var newY = Random.Range(-rectTransform.sizeDelta.y / 2, rectTransform.sizeDelta.y / 2);
            trashItem.transform.localPosition = new Vector3(newX, newY);

            spawnedClutter.Add(trashItem);
            clutterPositions.Add(trashItem.transform.localPosition);
        }
    }

    private void RestoreMiniGame()
    {
        uiTable.alpha = 1;
        uiTable.blocksRaycasts = true;
        uiTable.interactable = true;

        for (int i = 0; i < spawnedItems.Count; i++)
        {
            if (spawnedItems[i] != null)
            {
                spawnedItems[i].transform.localPosition = itemPositions[i];
            }
        }

        for (int i = 0; i < spawnedClutter.Count; i++)
        {
            if (spawnedClutter[i] != null)
            {
                spawnedClutter[i].transform.localPosition = clutterPositions[i];
            }
        }

        isMiniGameActive = true;
        Debug.Log("Мини-игра восстановлена. Продолжайте поиск.");
    }

    private void OnToggleMiniGame(InputAction.CallbackContext context)
    {
        if (isMiniGameActive)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;
            isMiniGameActive = false;

            if (dialogLogic != null && onToggleMiniGamePhrase.Count > 0)
            {
                var rndPhraseIndex = Random.Range(0, onToggleMiniGamePhrase.Count);
                dialogLogic.SetCurrentPhrase(onToggleMiniGamePhrase[rndPhraseIndex]);
            }

            Debug.Log("Мини-игра свернута. Вы можете продолжить диалог.");
        }
        else
        {
            if (isPlayerInZone && !isCompleted) // Проверяем, не завершена ли мини-игра
            {
                InitializeMiniGame();
            }
        }
    }

    public void FindItem(GameObject item)
    {
        counter++;
        spawnedItems.Remove(item);
        Destroy(item);

        Debug.Log($"Собрано {counter} из {totalItems} предметов");

        if (counter == totalItems)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;

            OnDisable();
            Debug.Log("Вы собрали все предметы!");

            // Выдача награды игроку
            RewardPlayer();

            // Блокировка взаимодействия с объектом
            isCompleted = true;
        }
    }

    private void RewardPlayer()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectItem(rewardItemId, rewardQuantity);
            Debug.Log($"Игрок получил {rewardQuantity} предмет(а/ов) с ID: {rewardItemId}");
        }
        else
        {
            Debug.LogError("GameManager не найден!");
        }
    }

    private void OnDisable()
    {
        if (toggleMiniGameAction != null)
        {
            toggleMiniGameAction.performed -= OnToggleMiniGame;
            toggleMiniGameAction.Disable();
        }

        spawnedItems.Clear();
        spawnedClutter.Clear();
        itemPositions.Clear();
        clutterPositions.Clear();

        isMiniGameInitialized = false;
        isMiniGameActive = false;
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

    public void SetPlayerInZone(bool inZone)
    {
        isPlayerInZone = inZone;
    }
}