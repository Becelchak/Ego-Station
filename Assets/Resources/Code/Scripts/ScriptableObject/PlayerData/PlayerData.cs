using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/PlayerData")]
public class PlayerData : ScriptableObject
{
    public Dictionary<string, int> itemQuantities = new Dictionary<string, int>();

    public void AddItem(string itemId, int quantity = 1)
    {
        if (itemQuantities.ContainsKey(itemId))
        {
            itemQuantities[itemId] += quantity;
        }
        else
        {
            itemQuantities.Add(itemId, quantity);
        }
    }

    public bool HasItem(string itemId)
    {
        return itemQuantities.ContainsKey(itemId);
    }

    public int GetItemQuantity(string itemId)
    {
        return itemQuantities.ContainsKey(itemId) ? itemQuantities[itemId] : 0;
    }

    public void RemoveItem(string itemId, int quantity = 1)
    {
        if (itemQuantities.ContainsKey(itemId))
        {
            itemQuantities[itemId] -= quantity;
            if (itemQuantities[itemId] <= 0)
            {
                itemQuantities.Remove(itemId);
            }
        }
    }

    public void ResetData()
    {
        itemQuantities.Clear();
    }

    // Метод для отображения предметов в инспекторе
    public void DisplayItems()
    {
        foreach (var item in itemQuantities)
        {
            Debug.Log($"Item ID: {item.Key}, Quantity: {item.Value}");
        }
    }
}