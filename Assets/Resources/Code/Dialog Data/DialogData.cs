using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialog Data")]
/// <summary>
/// �����, ���������� ������ � �������, ����� ��� ������ ���� ����.
/// </summary>
public class DialogData : ScriptableObject
{
    public List<DialogRequirement> requirements; // ���������� ��� ������ �������
    /// <summary>
    /// ������ ���� ����, ����������� � ������� �������.
    /// </summary>
    [SerializeField] private List<Phrase> allPhrases = new List<Phrase>();
    public Phrase startPhrase;

    /// <summary>
    /// ���������� ������ ���� ����, ������������ � �������.
    /// </summary>
    /// <returns>������ ���� ���� <see cref="Phrase"/>.</returns>
    public List<Phrase> GetPhrases()
    {
        return allPhrases;
    }
}
