using UnityEngine;

namespace SeriousGame.Runtime
{
    // Gắn script này vào UI/world-text để nó luôn quay mặt về camera (player)
    public class BillboardToCamera : MonoBehaviour
    {
        // Nếu để trống sẽ tự dùng Camera.main
        public Transform target;

        void LateUpdate()
        {
            Transform cam = target;
            if (cam == null && Camera.main != null)
            {
                cam = Camera.main.transform;
            }

            if (cam == null) return;

            // Hướng từ camera -> object
            Vector3 dir = transform.position - cam.position;
            if (dir.sqrMagnitude < 0.0001f) return;

            // Cho UI quay mặt về camera
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}
