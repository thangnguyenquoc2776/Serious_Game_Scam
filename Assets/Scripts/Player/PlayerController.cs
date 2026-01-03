using UnityEngine;


namespace SeriousGame.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        
        public float interactDistance = 2f;
        public LayerMask interactableLayer;

        [Header("Movement")]
        public float moveSpeed = 4f;

        [Header("Mouse Look")]
        public float mouseSensitivity = 2f;
        public Transform cameraHolder;

        private Rigidbody rb;
        private float xRotation = 0f;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            HandleMouseLook();
            HandleInteraction();
        }

        void FixedUpdate()
        {
            HandleMovement();
        }

        void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 moveDir =
                transform.right * h +
                transform.forward * v;

            Vector3 velocity = moveDir.normalized * moveSpeed;
            Vector3 targetPos = rb.position + velocity * Time.fixedDeltaTime;

            rb.MovePosition(targetPos);
        }

        void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -80f, 80f);

            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }

        void HandleInteraction()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                interactable?.Interact();
            }
        }

    }
}
