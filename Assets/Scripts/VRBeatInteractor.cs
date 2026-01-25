using SeriousGame.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class VRBeatInteractor : MonoBehaviour
{
    [Header("Ray source")]
    [SerializeField] private XRRayInteractor rayInteractor;

    [Header("Input (usually Trigger)")]
    [SerializeField] private InputActionReference interactAction;

    // [Header("Optional filtering")]
    // [SerializeField] private LayerMask interactMask = ~0;
    // [SerializeField] private float fallbackMaxDistance = 10f;

    private void OnEnable()
    {
        if (interactAction.action != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        if (interactAction.action != null)
            interactAction.action.performed -= OnInteractPerformed;
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        // 1) Prefer XRRayInteractor hit (matches the visible ray)
        if (rayInteractor != null && rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            TryCallInteract(hit);
            return;
        }

        // 2) Fallback: manual physics raycast from this transform forward
        // Ray ray = new Ray(transform.position, transform.forward);
        // if (Physics.Raycast(ray, out RaycastHit hit2, fallbackMaxDistance, interactMask, QueryTriggerInteraction.Ignore))
        // {
        //     TryCallInteract(hit2);
        // }
    }

    private void TryCallInteract(RaycastHit hit)
    {
        // Use GetComponentInParent because collider may be on a child
        var interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            interactable.Interact();
        }
        // else: nothing to interact with
    }
}
