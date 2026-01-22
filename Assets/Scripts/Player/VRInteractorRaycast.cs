using UnityEngine;
using UnityEngine.InputSystem;

namespace SeriousGame.Runtime
{
    /// <summary>
    /// Raycast từ controller (hoặc một Transform bất kỳ) ra thế giới,
    /// nếu trúng IInteractable thì gọi Interact().
    /// Mục tiêu: giữ y nguyên toàn bộ Episode/Beat logic hiện tại.
    /// </summary>
    public class VRInteractorRaycast : MonoBehaviour
    {
        [Header("Raycast Settings")]
        public float interactDistance = 3f;
        public LayerMask interactableLayer;

        [Header("Ray Origin (optional)")]
        [Tooltip("Nếu để trống sẽ dùng chính transform của GameObject này.")]
        public Transform rayOrigin;

        [Header("Debug")]
        [Tooltip("Chỉ để debug trong Editor, không bắt buộc.")]
        public bool drawDebugRay = true;

        [Header("Input System (tuỳ chọn)")]
        [Tooltip("Gán action (ví dụ XRI RightHand Activate) để bóp trigger là Interact.")]
        public InputActionReference interactAction;

        void Awake()
        {
            if (rayOrigin == null)
            {
                rayOrigin = transform;
            }
        }

        void OnEnable()
        {
            if (interactAction != null)
            {
                interactAction.action.performed += OnInteractAction;
                interactAction.action.Enable();
            }
        }

        void OnDisable()
        {
            if (interactAction != null)
            {
                interactAction.action.performed -= OnInteractAction;
                interactAction.action.Disable();
            }
        }

        void Update()
        {
            // Fallback cho lúc test trên PC chưa gắn input của XRI:
            // Nhấn E để bắn ray từ controller.
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteract();
            }
        }

        void OnInteractAction(InputAction.CallbackContext ctx)
        {
            // Được gọi khi action (trigger) được bóp
            TryInteract();
        }

        /// <summary>
        /// Hàm này bạn có thể gọi từ sự kiện input của XRI (trigger, grip...).
        /// Ví dụ: map vào action "Activate" hoặc "Select".
        /// </summary>
        public void TryInteract()
        {
            if (rayOrigin == null)
                return;

            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);

            if (drawDebugRay)
            {
                Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.cyan, 0f);
            }

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact();
            }
        }

        void OnDrawGizmosSelected()
        {
            if (rayOrigin == null)
                rayOrigin = transform;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(rayOrigin.position, rayOrigin.position + rayOrigin.forward * interactDistance);
        }
    }
}
