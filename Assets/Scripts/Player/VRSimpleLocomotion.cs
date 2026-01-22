using UnityEngine;

namespace SeriousGame.Runtime
{
    /// <summary>
    /// Di chuyển đơn giản cho XR Origin bằng WASD + quay Q/E.
    /// Dùng hướng nhìn của camera làm "forward" để cảm giác giống đi bộ.
    /// Không phụ thuộc XRI actions, phù hợp để test nhanh.
    /// </summary>
    public class VRSimpleLocomotion : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 2f;

        [Header("Turn (optional)")]
        public float turnSpeed = 90f; // độ/giây cho quay Q/E

        [Header("Head / Camera")]
        [Tooltip("Thường gán Main Camera (HMD)")]
        public Transform head;

        void Update()
        {
            HandleMove();
            HandleTurn();
        }

        void HandleMove()
        {
            float h = Input.GetAxisRaw("Horizontal");   // A/D hoặc mũi tên trái/phải
            float v = Input.GetAxisRaw("Vertical");     // W/S hoặc mũi tên lên/xuống

            if (Mathf.Approximately(h, 0f) && Mathf.Approximately(v, 0f))
                return;

            // Hướng forward lấy theo đầu (camera) nếu có, ngược lại lấy theo thân (transform)
            Vector3 forward = head != null ? head.forward : transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = new Vector3(forward.z, 0f, -forward.x);

            Vector3 moveDir = forward * v + right * h;
            if (moveDir.sqrMagnitude > 1f)
                moveDir.Normalize();

            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        void HandleTurn()
        {
            float turnInput = 0f;

            if (Input.GetKey(KeyCode.Q))
                turnInput -= 1f;
            if (Input.GetKey(KeyCode.E))
                turnInput += 1f;

            if (Mathf.Approximately(turnInput, 0f))
                return;

            float yaw = turnInput * turnSpeed * Time.deltaTime;
            transform.Rotate(0f, yaw, 0f, Space.World);
        }
    }
}
