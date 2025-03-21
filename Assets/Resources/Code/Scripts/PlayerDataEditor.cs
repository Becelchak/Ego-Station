using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerData))]
public class PlayerDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerData playerData = (PlayerData)target;

        if (playerData.itemQuantities != null && playerData.itemQuantities.Count > 0)
        {
            EditorGUILayout.LabelField("Items in Inventory:");
            foreach (var item in playerData.itemQuantities)
            {
                EditorGUILayout.LabelField($"Item ID: {item.Key}, Quantity: {item.Value}");
            }
        }
        else
        {
            EditorGUILayout.LabelField("Inventory is empty.");
        }

        if (GUILayout.Button("Reset Inventory"))
        {
            playerData.ResetData();
        }
    }
}