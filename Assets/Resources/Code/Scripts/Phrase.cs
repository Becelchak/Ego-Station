using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Dialog/ Phrase")]
public class Phrase : ScriptableObject
{
    public string characterName;
    public string textPhrase;
    public bool isChoise;
    public List<Choice> choises;
}
