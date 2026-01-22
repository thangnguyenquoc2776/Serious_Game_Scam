using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;  // gán Canvas world-space

    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Transform originalParent;
    private bool droppedOnSlot;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        droppedOnSlot = false;

        // kéo card lên top để không bị layout của parent đè
        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
{
    // Chuyển đổi vị trí chuột (screen point) sang tọa độ Local của Canvas
    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform, 
        eventData.position, 
        eventData.pressEventCamera, 
        out Vector2 localPoint))
    {
        // Gán trực tiếp vị trí local của card bằng vị trí local của chuột trên Canvas
        rect.localPosition = localPoint;
    }
}

    public void OnEndDrag(PointerEventData eventData)
    {
        // nếu không drop vào slot nào thì tự về parent cũ
        if (!droppedOnSlot)
        {
            transform.SetParent(originalParent);
        }
        canvasGroup.blocksRaycasts = true;
    }

    public void SetParentToSlot(Transform slot)
    {
        transform.SetParent(slot);
        rect.anchoredPosition = Vector2.zero;
        droppedOnSlot = true;
        canvasGroup.blocksRaycasts = true;
    }
}