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
    public GameObject dossier3;

    [Header("Camera zoom config")]
    public float focusDistance = 0.6f; // chỉnh ở Inspector



    void Awake()
    {
        cam = Camera.main.transform;
        root.SetActive(false);
    }

    public void ShowDossier(int index)
    {
        dossier1.SetActive(index == 0);
        dossier2.SetActive(index == 1);
        dossier3.SetActive(index == 2);
    }

    public void Show(BeatSO beat, InteractionSO interaction, Action onDone)
    {
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
        root.SetActive(false);

        // trả camera về
        cam.position = camOriginalPos;
        cam.rotation = camOriginalRot;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        player.SetLockState(false);

        onFinish?.Invoke(); // báo beat xong
    }
}
