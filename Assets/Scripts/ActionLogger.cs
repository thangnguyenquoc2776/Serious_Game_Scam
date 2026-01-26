using UnityEngine;
using UnityEngine.InputSystem;

public class ActionLogger : MonoBehaviour
{
    public InputActionReference clickAction;

    void OnEnable()
    {
        if (clickAction?.action != null)
            clickAction.action.performed += OnPerformed;
    }
    void OnDisable()
    {
        if (clickAction?.action != null)
            clickAction.action.performed -= OnPerformed;
    }
    void OnPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log($"[ActionLogger] performed: {ctx.action.name}, val={ctx.ReadValueAsObject()}, control={ctx.control}");
    }
}
