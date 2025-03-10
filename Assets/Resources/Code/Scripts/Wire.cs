using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Wire : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public RectTransform startPoint;
    public RectTransform endPoint;
    public Image wireImage;

    private Vector2 initialPosition;
    private WireMiniGame miniGame;
    private RectTransform wireRectTransform;
    private bool isConnected;

    private void Awake()
    {
        wireRectTransform = wireImage.GetComponent<RectTransform>();
    }

    public void Initialize(WireMiniGame game)
    {
        miniGame = game;
        initialPosition = startPoint.anchoredPosition;
        ResetWire();
    }

    public void SetEndPoint(RectTransform end)
    {
        endPoint = end;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        initialPosition = startPoint.anchoredPosition;
        if (isConnected)
        {
            miniGame.DisconnectWire();
            isConnected = false;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            wireRectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition
        );

        UpdateWire(localPointerPosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Vector2.Distance(eventData.position, endPoint.position) < miniGame.wireSnapDistance && !isConnected)
        {
            ConnectTo(endPoint);
            miniGame.CheckWireConnection(this, endPoint.anchoredPosition);
        }
        else
        {
            ResetWire();
            if (isConnected)
                miniGame.DisconnectWire();
        }

    }

    private void UpdateWire(Vector2 targetPosition)
    {
        var direction = targetPosition - startPoint.anchoredPosition;
        var distance = direction.magnitude;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        wireRectTransform.sizeDelta = new Vector2(distance, wireRectTransform.sizeDelta.y);
        wireRectTransform.anchoredPosition = startPoint.anchoredPosition + direction / 2;
        wireRectTransform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void ConnectTo(RectTransform target)
    {
        isConnected = true;
    }

    public void ResetWire()
    {
        wireRectTransform.sizeDelta = new Vector2(0, wireRectTransform.sizeDelta.y);
        wireRectTransform.anchoredPosition = startPoint.anchoredPosition;
        wireRectTransform.localEulerAngles = Vector3.zero;
    }
}