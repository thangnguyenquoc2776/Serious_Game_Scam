using System.Collections.Generic;
using UnityEngine;
using SeriousGame.Runtime;
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

        [Header("Camera Mode")]
        public Camera thirdPersonCamera;
        public Camera firstPersonCamera;

        [Header("Seating")]
        public bool allowLookWhileSeated = true;
        public bool allowInteractWhileSeated = false;
        public bool lockMovementWhileSeated = true;

        private Rigidbody rb;
        private float xRotation = 0f;
        // Trong PlayerController.cs
        public bool isLocked = false;
        private const string LegacyLockSource = "Legacy";
        private readonly HashSet<string> softLockSources = new HashSet<string>();
        private readonly HashSet<string> hardLockSources = new HashSet<string>();
        private bool isSeated = false;
        private Vector3 originalCameraLocalPos;
        private Vector3 lastSeatedCameraOffset;

        Animator animator;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            animator = GetComponentInChildren<Animator>();

            if (cameraHolder != null)
                originalCameraLocalPos = cameraHolder.localPosition;
        }

        void Update()
        {   
            if (isLocked) return;

            if (!isSeated || allowLookWhileSeated)
                HandleMouseLook();

            if (!isSeated || allowInteractWhileSeated)
                HandleInteraction();
        }

        // Thêm hàm để điều khiển chuột
        public void SetLockState(bool locked)
        {
            SetLockState(LegacyLockSource, locked);
        }

        public void SetHardLock(bool locked)
        {
            SetHardLock(LegacyLockSource, locked);
        }

        public void SetLockState(string source, bool locked)
        {
            UpdateLockSources(softLockSources, source, locked);
            ApplyLockState();
        }

        public void SetHardLock(string source, bool locked)
        {
            UpdateLockSources(hardLockSources, source, locked);
            ApplyLockState();
        }

        private void UpdateLockSources(HashSet<string> sources, string source, bool locked)
        {
            if (string.IsNullOrWhiteSpace(source))
                source = LegacyLockSource;

            if (locked)
                sources.Add(source);
            else
                sources.Remove(source);
        }

        private void ApplyLockState()
        {
            var shouldLock = softLockSources.Count > 0 || hardLockSources.Count > 0;
            Debug.Log($"[PlayerController] SetLockState: {shouldLock} (soft={softLockSources.Count}, hard={hardLockSources.Count})");
            isLocked = shouldLock;
            Cursor.lockState = shouldLock ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = shouldLock;
            if (rb != null)
                rb.linearVelocity = Vector3.zero; // Dừng lập tức

            // Khi khoá điều khiển, đảm bảo Animator không còn trạng thái đi bộ
            if (shouldLock && animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }
        void FixedUpdate()
        {   
            if (isLocked) return;
            if (isSeated && lockMovementWhileSeated) return;
            HandleMovement();
        }

        void HandleMovement()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 moveDir =
                transform.right * h +
                transform.forward * v;

            // Cập nhật trạng thái đi bộ cho Animator dựa trên input
            if (animator != null)
            {
                bool isMoving = moveDir.sqrMagnitude > 0.01f;
                animator.SetBool("isWalking", isMoving);
            }

            // Không có input thì không cần di chuyển
            if (moveDir.sqrMagnitude <= 0.01f)
                return;

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
            if (Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
                {
                    var interactable = hit.collider.GetComponent<IInteractable>();
                    interactable?.Interact();
                }
            }
        }

        public void Sit(Transform seat, Vector3 positionOffset, Vector3 rotationOffsetEuler, Vector3 cameraLocalOffset)
        {
            if (seat == null) return;

            if (!isSeated)
            {
                if (rb != null)
                    lastSeatedCameraOffset = Vector3.zero;
                if (cameraHolder != null)
                    originalCameraLocalPos = cameraHolder.localPosition;
            }

            isSeated = true;
            lastSeatedCameraOffset = cameraLocalOffset;

            var worldPos = seat.TransformPoint(positionOffset);
            if (rb != null)
                rb.position = worldPos;
            else
                transform.position = worldPos;

            transform.rotation = seat.rotation * Quaternion.Euler(rotationOffsetEuler);

            if (rb != null)
                rb.linearVelocity = Vector3.zero;

            if (cameraHolder != null)
                cameraHolder.localPosition = originalCameraLocalPos + cameraLocalOffset;

            SetCameraMode(true);
        }

        public void UnSit()
        {
            if (!isSeated) return;

            isSeated = false;
            if (cameraHolder != null)
                cameraHolder.localPosition = originalCameraLocalPos;

            SetCameraMode(false);
        }

        private void SetCameraMode(bool firstPerson)
        {
            if (thirdPersonCamera != null)
                thirdPersonCamera.enabled = !firstPerson;
            if (firstPersonCamera != null)
                firstPersonCamera.enabled = firstPerson;
        }

        // Teleport player tới một vị trí/rotation định trước (ví dụ: phòng office)
        public void TeleportTo(Transform target)
        {
            if (target == null || rb == null) return;

            // Đặt lại vị trí ngay lập tức
            rb.position = target.position;

            // Xoay player theo hướng của target (chỉ yaw)
            Vector3 euler = target.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, euler.y, 0f);

            // Dừng chuyển động
            rb.linearVelocity = Vector3.zero;
        }

    }
}