using UnityEngine;
using UnityEngine.UI;
using System;
using static PlayerManager;
using System.Runtime.InteropServices;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerData playerData;
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private DialogLogic endGameDialogLogic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (playerData == null)
        {
            Debug.LogError("PlayerData not assigned!");
        }
    }

    public void CollectItem(string itemId, int quantity = 1)
    {
        playerData.AddItem(itemId, quantity);
    }

    public bool CheckIfItemCollected(string itemId)
    {
        return playerData.HasItem(itemId);
    }

    public void RemoveItemFromInventory(string itemId, int quantity = 1)
    {
        playerData.RemoveItem(itemId, quantity);
    }

    public void ResetPlayerData()
    {
        playerData.ResetData();
    }

    public void EndingInitialization(PlayerManager playerManager)
    {

        var maxAttribute = PlayerAttributes.Body;
        double maxValue = playerManager.BodyAttribute;

        if (playerManager.MindAttribute > maxValue)
        {
            maxValue = playerManager.MindAttribute;
            maxAttribute = PlayerAttributes.Mind;
        }

        if (playerManager.FeelsAttribute > maxValue)
        {
            maxValue = playerManager.FeelsAttribute;
            maxAttribute = PlayerAttributes.Feels;
        }

        switch (maxAttribute)
        {
            case PlayerManager.PlayerAttributes.Body:
                var bodyDialogData = Resources.Load<DialogData>("Code/Dialog Data/Ending/Body/BodyEnding");
                Debug.Log($"{bodyDialogData}");
                endGameDialogLogic.SetDialogData(bodyDialogData);
                break;
            case PlayerManager.PlayerAttributes.Mind:
                var mindDialogData = Resources.Load<DialogData>("Code/Dialog Data/Ending/Mind/MindEnding");
                endGameDialogLogic.SetDialogData(mindDialogData);
                break;
            case PlayerManager.PlayerAttributes.Feels:
                var feelsDialogData = Resources.Load<DialogData>("Code/Dialog Data/Ending/Feels/FeelsEnding");
                endGameDialogLogic.SetDialogData(feelsDialogData);
                break;
        }

        endGameDialogLogic.gameObject.SetActive(true);
        Instantiate(backgroundImage,GameObject.Find("Dialog UI canvas").transform);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.name == "Player")
        {
            var playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
            EndingInitialization(playerManager);
        }
    }
}