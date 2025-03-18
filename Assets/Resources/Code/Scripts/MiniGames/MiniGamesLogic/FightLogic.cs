using EventBusSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightLogic : Logic
{
    [SerializeField] private Transform playerPoint;
    [SerializeField] private Transform enemyPoint;

    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject player;
    private PlayerManager playerHP;

    private bool enemiIsAlive;
    private GameObject enemy;
    //private InputAction attackAction;

    public TextMeshProUGUI keyDisplayText;
    public float reactionTime = 1f;
    private bool isWaitingForInput = false;
    private KeyCode targetKey;

    private CanvasGroup panel;

    void Start()
    {
        StartCoroutine(StartQTE());
        player.transform.position = playerPoint.position;
        playerHP = player.GetComponent<PlayerManager>();
        panel = GameObject.Find("Fight panel").GetComponent<CanvasGroup>();
    }

    void Update()
    {  
        if(enemies.Count > 0 && !enemiIsAlive)
        {
            enemies[0].transform.position = enemyPoint.position;
            enemy = enemies[0];
            enemiIsAlive = true;
        }

        if (isWaitingForInput && Input.anyKeyDown)
        {
            if (Input.GetKeyDown(targetKey))
            {
                Debug.Log("Правильно! Нажата клавиша: " + targetKey);
                enemiIsAlive = false;
                enemies.Remove(enemy);
                Destroy(enemy);

                StopAllCoroutines();

                if (enemies.Count == 0)
                {
                    panel.alpha = 0;
                    panel.interactable = false;
                    panel.blocksRaycasts = false;
                    this.enabled = false;
                }
                StartCoroutine(StartQTE());
            }
            else
            {
                StopAllCoroutines();
                Debug.Log("Ошибка! Нужно было нажать: " + targetKey);
                EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(15));
                Debug.Log($"Здоровье равно {playerHP.Health}");
                StartCoroutine(StartQTE());
            }
        }
    }

    IEnumerator StartQTE()
    {
        while (isActiveAndEnabled)
        {
            isWaitingForInput = false;
            keyDisplayText.text = "";
            yield return new WaitForSeconds(1f);

            targetKey = GetRandomKey();
            keyDisplayText.text = "Press: " + targetKey.ToString();
            isWaitingForInput = true;

            yield return new WaitForSeconds(reactionTime);

            if (isWaitingForInput)
            {
                Debug.Log("Время вышло! Нужно было нажать: " + targetKey);
                EventBus.RaiseEvent<IPlayerSubscriber>(h => h.GetDamage(15));
                Debug.Log($"Здоровье равно {playerHP.Health}");
            }
        }
    }
    private KeyCode GetRandomKey()
    {
        KeyCode[] keys = { KeyCode.T, KeyCode.Y, KeyCode.R, KeyCode.G};
        return keys[Random.Range(0, keys.Length)];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            player = other.gameObject;
        else if (other.tag == "NPC" && !enemies.Contains(other.gameObject))
            enemies.Add(other.gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "NPC" && !enemies.Contains(other.gameObject))
            enemies.Add(other.gameObject);
    }
}
