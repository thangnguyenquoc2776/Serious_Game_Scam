using UnityEngine;
using UnityEngine.EventSystems;

public enum SlotType { Cause, Solution }

public class DropSlot : MonoBehaviour, IDropHandler
{
    public SlotType slotType;
    public int index; // dòng thứ mấy (0,1,2,...)

    public void OnDrop(PointerEventData eventData)
    {
        var card = eventData.pointerDrag?.GetComponent<Dragable>();
        if (card == null) return;

        card.SetParentToSlot(transform);
    }
}