using UnityEngine;

namespace SeriousGame.Runtime
{
    public class PlayerController : MonoBehaviour
    {   
    
        public float interactDistance = 2f;
        public LayerMask interactableLayer;
        public bool isSitting = false;

        [Header("Movement")]
        public float moveSpeed = 4f;

        [Header("Mouse Look")]
        public float mouseSensitivity = 2f;
        public Transform cameraHolder;

        private Rigidbody rb;
        private float xRotation = 0f;
        // Trong PlayerController.cs
        public bool isLocked = false;

        Animator animator;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {   
            if (isLocked) return;
            HandleMouseLook();
            HandleInteraction();
            HandleSitting();
        }

        // Thêm hàm để điều khiển chuột
        public void SetLockState(bool locked)
        {
            isLocked = locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
            rb.linearVelocity = Vector3.zero; // Dừng lập tức

            // Khi khoá điều khiển, đảm bảo Animator không còn trạng thái đi bộ
            if (locked && animator != null)
            {
                animator.SetBool("isWalking", false);
            }
        }
        void FixedUpdate()
        {   
            if (isLocked) return;
            HandleMovement();
        }

        void HandleMovement()
        {   if (!isSitting)  // if is sitting then don't move
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

        private void blockMovement()
        {
            rb.linearVelocity = Vector3.zero;
        }

        

        // trong class PlayerController:
        [HideInInspector] public bool isSittingByBeat = false;
        private Vector3 originalCameraLocalPos;

        // gọi khi muốn "ngồi" do beat điều khiển
        public void SitAt(Transform sitAnchor)
        {
            if (sitAnchor == null) return;

            // teleport player tới vị trí ghế
            TeleportTo(sitAnchor);

            // lưu vị trí camera ban đầu nếu chưa lưu
            if (originalCameraLocalPos == Vector3.zero)
                originalCameraLocalPos = cameraHolder.localPosition;

            // chỉnh camera một chút để cảm giác ngồi (tweak giá trị theo scene)
            cameraHolder.localPosition = originalCameraLocalPos + new Vector3(0f, -0.2f, 1.5f);

            isSitting = true;
            isSittingByBeat = true;

            // khoá/đóng băng control nếu cần (nhưng vẫn cho phép look nếu bạn muốn)
            SetLockState(false); // giữ mouse look; nếu muốn khoá cả mouse, SetLockState(true);
        }

        // gọi để đứng dậy (do F hoặc do beat muốn đứng)
        public void StandUp()
        {
            isSitting = false;
            isSittingByBeat = false;

            // khôi phục camera
            if (originalCameraLocalPos != Vector3.zero)
                cameraHolder.localPosition = originalCameraLocalPos;

            SetLockState(false);
        }

        public void HandleSitting()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                // nếu đang ngồi do beat -> gọi StandUp cho consistent
                if (isSitting && isSittingByBeat)
                {
                    StandUp();
                    return;
                }

                isSitting = !isSitting;
                if (isSitting)
                {
                    // local fallback: ngồi tại chỗ (không teleport)
                    if (originalCameraLocalPos == Vector3.zero)
                        originalCameraLocalPos = cameraHolder.localPosition;
                    cameraHolder.localPosition += new Vector3(0, 0, 2f);
                }
                else
                {
                    cameraHolder.localPosition = originalCameraLocalPos;
                }
            }
        }



    }
}