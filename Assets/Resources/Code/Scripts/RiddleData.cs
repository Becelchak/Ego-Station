using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/RiddleData")]
public class RiddleData : ScriptableObject
{
    [System.Serializable]
    public class Riddle
    {
        public string Question;
        public List<Answer> Answers;
    }

    [System.Serializable]
    public class Answer
    {
        public string Text;
        public bool IsCorrect;
    }

    public List<Riddle> Riddles;
    public Phrase SuccessPhrase;
    public Phrase FailPhrase;
}