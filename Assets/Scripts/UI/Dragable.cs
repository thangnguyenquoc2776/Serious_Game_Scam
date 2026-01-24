using UnityEngine;
using UnityEngine.EventSystems;

public class Dragable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Canvas canvas;  // gán Canvas world-space (monitor canvas)

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
        transform.SetParent(canvas.transform, worldPositionStays: true);
        transform.SetAsLastSibling();

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        RectTransform canvasRect = canvas.transform as RectTransform;

        // ✅ Ưu tiên worldPosition (chuẩn cho XR ray)
        Vector3 worldPos = eventData.pointerCurrentRaycast.worldPosition;

        if (worldPos != Vector3.zero)
        {
            Vector3 local = canvasRect.InverseTransformPoint(worldPos);
            rect.localPosition = new Vector3(local.x, local.y, rect.localPosition.z);
            return;
        }

        // Fallback cho PC/mouse (nếu cần)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            rect.localPosition = new Vector3(localPoint.x, localPoint.y, rect.localPosition.z);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!droppedOnSlot)
        {
            transform.SetParent(originalParent, worldPositionStays: true);
        }

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }

    public void SetParentToSlot(Transform slot)
    {
        transform.SetParent(slot, worldPositionStays: false);
        rect.anchoredPosition = Vector2.zero;
        droppedOnSlot = true;

        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;
    }
}
