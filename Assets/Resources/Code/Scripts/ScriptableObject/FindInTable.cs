using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[CreateAssetMenu(menuName = "Dialog/ DialogEvent/ FindInTable")]
public class FindInTable : DialogEvent
{
    [SerializeField] private CanvasGroup uiTable;
    private int counter;
    private int totalItems;
    public override void Raise()
    {
        counter = 0;

        uiTable = GameObject.Find("FindTable canvas").GetComponent<CanvasGroup>();
        uiTable.alpha = 1;
        uiTable.blocksRaycasts = true;
        uiTable.interactable = true;

        var buttons = uiTable.transform.GetComponentsInChildren<Button>();
        totalItems = buttons.Length;

        foreach(var button in buttons)
        {
            button.onClick.AddListener(() => { FindItem(button); });
        }
    }

    public void FindItem(Button button)
    {
        counter++;

        Destroy(button.gameObject);

        Debug.Log($"Собрано {counter} из {totalItems} фрагментов");

        if (counter == totalItems)
        {
            uiTable.alpha = 0;
            uiTable.blocksRaycasts = false;
            uiTable.interactable = false;

            Debug.Log("Вы собрали огнемет!");
        }
    }

    public void Update()
    {
        
    }
}
