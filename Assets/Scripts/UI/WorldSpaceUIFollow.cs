using UnityEngine;

public class WorldSpaceUIFollow : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("Nếu để trống, script sẽ dùng Camera.main.")]
    public Transform targetCamera;

    [Header("Follow Settings")]
    [Tooltip("Khoảng cách UI trước mặt camera (m).")]
    public float distance = 1.2f;

    [Tooltip("Offset theo local space của camera (m). Ví dụ hạ UI xuống một chút.")]
    public Vector3 cameraLocalOffset = new Vector3(0f, -0.15f, 0f);

    [Tooltip("Nếu bật, UI sẽ follow mượt thay vì teleport theo camera.")]
    public bool smoothFollow = true;

    [Tooltip("Tốc độ follow mượt (càng lớn càng bám chặt).")]
    public float positionLerpSpeed = 12f;

    [Header("Rotation Settings")]
    [Tooltip("Nếu true: UI chỉ xoay theo yaw (trục Y). Khuyên dùng cho VR.")]
    public bool yawOnly = true;

    [Tooltip("Tốc độ xoay mượt.")]
    public float rotationLerpSpeed = 12f;

    [Header("Update Timing")]
    [Tooltip("LateUpdate thường ổn hơn vì camera đã update xong trong frame.")]
    public bool useLateUpdate = true;

    private void OnEnable()
    {
        if (targetCamera == null && Camera.main != null)
            targetCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (!useLateUpdate) return;
        Tick();
    }

    private void Update()
    {
        if (useLateUpdate) return;
        Tick();
    }

    private void Tick()
    {
        if (targetCamera == null)
        {
            if (Camera.main == null) return;
            targetCamera = Camera.main.transform;
        }

        // 1) Tính vị trí mong muốn: trước mặt camera + offset
        Vector3 desiredPos = targetCamera.position
                             + targetCamera.forward * distance
                             + targetCamera.TransformVector(cameraLocalOffset);

        // 2) Follow vị trí
        if (smoothFollow)
            transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-positionLerpSpeed * Time.deltaTime));
        else
            transform.position = desiredPos;

        // 3) Quay mặt về camera
        Vector3 lookDir = transform.position - targetCamera.position;

        if (yawOnly)
        {
            // chỉ xoay theo Y (bỏ pitch/roll)
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude < 0.0001f) return;

            Quaternion desiredRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime));
        }
        else
        {
            // xoay full theo hướng camera
            if (lookDir.sqrMagnitude < 0.0001f) return;

            Quaternion desiredRot = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-rotationLerpSpeed * Time.deltaTime));
        }
    }
}
