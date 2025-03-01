using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Dialog/ Phrase")]
public class Phrase : ScriptableObject
{
    [SerializeField] private string _characterName;
    [SerializeField] private string _textPhrase;
    [SerializeField] private bool _isChoise;
    [SerializeField] private List<Choice> _choises;
    [SerializeField] private Phrase nextPhrase;

    public string CharacterName => _characterName;
    public string TextPhrase => _textPhrase;
    public bool IsChoise => _isChoise;
    public List<Choice> Choises => _choises;
    public Phrase NextPhrase => nextPhrase;
}
