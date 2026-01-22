using SeriousGame.Content;
using SeriousGame.Runtime;
using System;
using UnityEngine;

public class PCUI : MonoBehaviour
{
    public GameObject root;            // Canvas world-space (đã gắn sẵn trong scene)
    public PlayerController player;

    [Header("Camera focus")]
    public Transform cameraFocusPoint; // Empty đặt trước màn hình
    public float focusSpeed = 4f;

    private Action onFinish;
    private Transform cam;
    private Vector3 camOriginalPos;
    private Quaternion camOriginalRot;

    [Header("Dossier Panels")]
    public GameObject dossier1;
    public GameObject dossier2;

    [Header("Camera zoom config")]
    public float focusDistance = 0.6f; // chỉnh ở Inspector

    public GameObject dossierPanel;
    public GameObject reportPanel;



    void Awake()
    {
        cam = Camera.main.transform;
        root.SetActive(false);
        dossierPanel.SetActive(false);
        reportPanel.SetActive(false);

    }

    public void ShowDossier(int index)
    {
        if (index == 1)
        {
            dossier1.SetActive(true);
            dossier2.SetActive(false);
        }
        else if (index == 2)
        {
            dossier1.SetActive(false);
            dossier2.SetActive(true);
        }
    }

    public void Show(BeatSO beat, InteractionSO interaction, Action onDone)
    {   
        if (beat.beatId == "B04"|| beat.beatId =="B05")
        {
            dossierPanel.SetActive(true);
            reportPanel.SetActive(false);
        }
        else if (beat.beatId == "B08")
        {
            dossierPanel.SetActive(false);
            reportPanel.SetActive(true);
        }
        onFinish = onDone;

        // Lưu camera ban đầu
        camOriginalPos = cam.position;
        camOriginalRot = cam.rotation;

        root.SetActive(true);

        // khóa movement
        player.SetLockState(true);

        // bật cursor (PC mode)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!root.activeSelf) return;

        Vector3 targetPos =
            cameraFocusPoint.position
            - cameraFocusPoint.forward * focusDistance;

        cam.position = Vector3.Lerp(
            cam.position,
            targetPos,
            Time.deltaTime * focusSpeed
        );

        cam.rotation = Quaternion.Slerp(
            cam.rotation,
            cameraFocusPoint.rotation,
            Time.deltaTime * focusSpeed
        );
    }


    public void OnFinishClicked()
    {   
        Debug.Log("[PCUI] OnFinishClicked");
        root.SetActive(false);

        // trả camera về
        cam.position = camOriginalPos;
        cam.rotation = camOriginalRot;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.SetLockState(false);

        onFinish?.Invoke(); // báo beat xong
    }

    // Nút "Send" cho các màn PC đặc biệt (ví dụ gửi báo cáo)
    // Gắn OnClick của button Send vào hàm này thay vì OnFinishClicked.
    // Nếu chưa cần xử lý gì đặc biệt, có thể gọi lại OnFinishClicked().
    public void OnSendClicked()
    {
        Debug.Log("[PCUI] OnSendClicked");

        // TODO: xử lý nội dung report / drag-drop ở đây (nếu có)

        // Sau khi xử lý xong, kết thúc interaction giống nút Done:
        OnFinishClicked();
    }
}
