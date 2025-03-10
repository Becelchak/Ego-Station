using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Unity.VisualScripting;

public class WireMiniGame : MonoBehaviour
{
    [SerializeField] private GameObject wirePrefab;
    [SerializeField] private Transform wireContainer;
    [SerializeField] private int wireCount = 4;
    [SerializeField] public float wireSnapDistance = 50f;

    private List<Wire> wires = new List<Wire>();
    private int connectedWires = 0;

    public delegate void OnMiniGameComplete(bool success);
    public event OnMiniGameComplete MiniGameComplete;

    private void OnDisable()
    {
        Debug.Log("All clear");
        foreach (var wire in wires)
        {
            Destroy(wire.gameObject);
            connectedWires = 0;
        }
        wires.Clear(); 
    }
    private void OnEnable()
    {
        GenerateWires();
    }
    private void GenerateWires()
    {
        for (int i = 0; i < wireCount; i++)
        {
            var wireObj = Instantiate(wirePrefab, wireContainer);
            var wire = wireObj.GetComponent<Wire>();

            var r = Random.Range(0f, 1f);
            var g = Random.Range(0f, 1f);
            var b = Random.Range(0f, 1f);

            wire.wireImage.color = new Color(r,g,b);

            wire.Initialize(this);
            wires.Add(wire);
        }

        ShuffleWireEnds();
    }

    private void ShuffleWireEnds()
    {
        List<RectTransform> endPoints = new List<RectTransform>();

        foreach (var wire in wires)
        {
            endPoints.Add(wire.endPoint);
        }

        for (int i = 0; i < endPoints.Count; i++)
        {
            var temp = endPoints[i];
            var randomIndex = Random.Range(i, endPoints.Count);
            endPoints[i] = endPoints[randomIndex];
            endPoints[randomIndex] = temp;
        }

        for (int i = 0; i < wires.Count; i++)
        {
            var rndIndex = Random.Range(0, endPoints.Count);
            wires[i].SetEndPoint(endPoints[rndIndex]);
            wires[i].endPoint.GetComponent<Image>().color = wires[i].wireImage.color;
            endPoints.Remove(endPoints[rndIndex]);
        }
    }

    public void CheckWireConnection(Wire draggedWire, Vector2 dropPosition)
    {
        foreach (var wire in wires)
        {
            if (wire != draggedWire && Vector2.Distance(wire.endPoint.anchoredPosition, dropPosition) < wireSnapDistance)
            {
                draggedWire.ConnectTo(wire.endPoint);
                connectedWires++;

                if (connectedWires >= wireCount)
                {
                    MiniGameComplete?.Invoke(true);
                }

                return;
            }
        }

        draggedWire.ResetWire();
    }

    public void DisconnectWire()
    {
        connectedWires--;
    }
}