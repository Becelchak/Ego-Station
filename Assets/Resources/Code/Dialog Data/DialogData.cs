using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Data")]
/// <summary>
/// Класс, содержащий данные о диалоге, такие как список всех фраз.
/// </summary>
public class DialogData : ScriptableObject
{
    public List<DialogRequirement> requirements; // Требования для начала диалога
    /// <summary>
    /// Список всех фраз, относящихся к данному диалогу.
    /// </summary>
    [SerializeField] private List<Phrase> allPhrases = new List<Phrase>();
    public Phrase startPhrase;

    /// <summary>
    /// Возвращает список всех фраз, содержащихся в диалоге.
    /// </summary>
    /// <returns>Список фраз типа <see cref="Phrase"/>.</returns>
    public List<Phrase> GetPhrases()
    {
        return allPhrases;
    }
}
