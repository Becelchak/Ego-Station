using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using EventBusSystem;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class MorseQTE : MonoBehaviour, IMorseQTEFinishSubscriber
{
    [Header("UI References")]
    [SerializeField] private GameObject morseCanvas;
    [SerializeField] private TextMeshProUGUI morseDisplay;
    [SerializeField] private TextMeshProUGUI morseTranslaitedDisplay;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private AudioClip morseClipShort;
    [SerializeField] private AudioClip morseClipLong;
    private StringBuilder morseBuilder;
    private AudioSource audioSource;
    
    [Header("Game Settings")]
    [SerializeField] private List<KeyCode> availableKeys = new List<KeyCode> { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };
    [SerializeField] private float timeBetweenSymbols = 1.5f;
    [SerializeField] private int totalSymbols = 22;

    [Header("Events")]
    [SerializeField] private List<DialogEvent> successEvents;
    [SerializeField] private DialogEvent failEvent;

    private KeyCode currentKey;
    private List<string> morseSequence = new List<string>();
    private int currentSymbolIndex = 0;
    private bool isActive = false;
    private Coroutine gameCoroutine;

    private readonly Dictionary<char, string> morseAlphabet = new Dictionary<char, string>
    {
        {'п', ".--. "}, {'р', ".-. "}, {'о', "--- "},
        {'ш', "---- "}, {'у', "..- "}                 
    };

    private readonly Dictionary<string, char> morseToChar = new Dictionary<string, char>()
    {
        {".--. ", 'п'}, {".-. ", 'р'}, {"--- ", 'о'},
        {"---- ", 'ш'}, {"..- ", 'у'}                
    };

    private void OnEnable()
    {
        EventBus.Subscribe(this);
        morseBuilder = new StringBuilder();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
        if (gameCoroutine != null)
            StopCoroutine(gameCoroutine);
    }

    public void Initialize()
    {
        if (isActive) return;

        morseSequence.Clear();
        currentSymbolIndex = 0;
        isActive = true;

        GenerateMorseSequence("прошу");
        EventBus.RaiseEvent<IMoveControllerSubscriber>(s => s.Freeze());
        gameCoroutine = StartCoroutine(RunMorseQTE());
    }

    private void GenerateMorseSequence(string word)
    {
        foreach (char letter in word.ToLower())
        {
            if (morseAlphabet.TryGetValue(letter, out string code))
            {
                morseSequence.AddRange(code.Select(c => c.ToString()));
            }
        }
    }

    private IEnumerator RunMorseQTE()
    {
        while (currentSymbolIndex < morseSequence.Count && currentSymbolIndex < totalSymbols)
        {
            var newCurrentKey = availableKeys[Random.Range(0, availableKeys.Count)];
            while (currentKey == newCurrentKey)
            {
                newCurrentKey = availableKeys[Random.Range(0, availableKeys.Count)];
                
            }
            currentKey = newCurrentKey;
            instructionText.text = $"Press {currentKey}!";

            bool pressedCorrectly = false;
            float elapsed = 0f;

            while (elapsed < timeBetweenSymbols && !pressedCorrectly)
            {
                
                if (Input.GetKeyDown(currentKey))
                {
                    pressedCorrectly = true;
                    AddMorseSymbol(morseSequence[currentSymbolIndex]);
                    currentSymbolIndex++;

                    if (currentSymbolIndex % 4 == 0 && currentSymbolIndex < morseSequence.Count)
                    {
                        yield return new WaitForSeconds(0.8f);
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.3f);
                    }
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (!pressedCorrectly)
            {
                FailGame();
                yield break;
            }

        }

        CompleteGame();
    }

    private void AddMorseSymbol(string symbol)
    {
        morseBuilder.Append(symbol);
        morseDisplay.text = morseBuilder.ToString();
        if(symbol == ".")
            audioSource.clip = morseClipShort;
        else audioSource.clip = morseClipLong;
        audioSource.Play();

        CheckForCompleteLetter();
    }

    private void CheckForCompleteLetter()
    {
        string currentMorse = morseBuilder.ToString();

        int lastSpaceIndex = currentMorse.LastIndexOf(' ');

        if (lastSpaceIndex >= 0)
        {
            string lastSequence = currentMorse.Substring(lastSpaceIndex + 1);

            for (int length = Mathf.Min(4, lastSequence.Length); length >= 1; length--)
            {
                string testSequence = lastSequence.Substring(lastSequence.Length - length, length);

                if (morseToChar.TryGetValue(testSequence + " ", out char letter))
                {
                    UpdateTranslation(currentMorse, letter);
                    return;
                }
            }
        }

        UpdateTranslation(currentMorse, '?');
    }


    private void UpdateTranslation(string fullMorse, char translatedLetter)
    {
        string[] letters = fullMorse.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        StringBuilder translation = new StringBuilder();

        foreach (string letterCode in letters)
        {
            if (morseToChar.TryGetValue(letterCode + " ", out char letter))
            {
                translation.Append(letter);
            }
            else
            {
                bool found = false;
                for (int len = letterCode.Length; len > 0; len--)
                {
                    string part = letterCode.Substring(0, len);
                    if (morseToChar.TryGetValue(part + " ", out char partialLetter))
                    {
                        translation.Append(partialLetter);
                        found = true;
                        break;
                    }
                }
                if (!found) translation.Append('?');
            }
        }

        morseTranslaitedDisplay.text = translation.ToString();
    }

    private void CompleteGame()
    {
        foreach (var successEvent in successEvents)
        {
            successEvent?.Raise();
        }
        EventBus.RaiseEvent<IMorseQTEFinishSubscriber>(s => s.OnMorseQTEFinished(true));
        EventBus.RaiseEvent<IGenerator>(s => s.TurnOn());
        CleanUp();
        Debug.Log($"succsess");
    }

    private void FailGame()
    {
        failEvent?.Raise();
        EventBus.RaiseEvent<IMorseQTEFinishSubscriber>(s => s.OnMorseQTEFinished(false));
        CleanUp();
        Debug.Log($"fail");
    }

    private void CleanUp()
    {
        isActive = false;
        morseCanvas.SetActive(false);
    }

    public void OnMorseQTEFinished(bool success)
    {
        EventBus.RaiseEvent<IMoveControllerSubscriber>(s => s.Unfreeze());
        gameObject.SetActive(false);
    }
}

public interface IMorseQTEFinishSubscriber : IGlobalSubscriber
{
    void OnMorseQTEFinished(bool success);
}