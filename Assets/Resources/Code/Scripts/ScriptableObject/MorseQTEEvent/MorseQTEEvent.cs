using EventBusSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "MorseQTEEvent", menuName = "Dialog Events/New Morse QTE Event")]
public class MorseQTEEvent : DialogEvent
{
    [SerializeField] private GameObject morsePrefab;
    [SerializeField] private string prefabParentName;
    public override void Raise()
    {
        var plauerUI = GameObject.Find(prefabParentName);
        var morseGame = Instantiate(morsePrefab).GetComponent<MorseQTE>();
        morseGame.transform.parent = plauerUI.transform;
        morseGame.Initialize();
    }

    public override void SetDialogLogic(DialogLogic logic)
    {

    }
}