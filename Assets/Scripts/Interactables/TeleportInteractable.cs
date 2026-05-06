using UnityEngine;
using SeriousGame.App;
using SeriousGame.Runtime;

// Interactable dùng để teleport player tới một vị trí (vd: qua cổng -> vào office)
// Có thể kích hoạt bằng nhấn E (IInteractable) hoặc tự động khi player bước vào trigger.
public class TeleportInteractable : MonoBehaviour
{
    [Header("Player & Destination")]
    public PlayerController player;      // kéo PlayerController vào đây
    public Transform destinationPoint;   // Empty ở phòng office

    [Header("Điều kiện (tuỳ chọn)")]
    [Tooltip("Nếu có, chỉ cho teleport khi flag này thoả điều kiện.")]
    public string requiredFlagKey;       // ví dụ: "PASSED_GUARD"
    public bool requiredValue = true;

    [Header("One-shot?")]
    public bool oneShot = true;
    private bool _used = false;

    public void Interact()
    {
        // Kích hoạt khi player nhấn E (thông qua PlayerController)
        TryTeleport(player);
    }

    void OnTriggerEnter(Collider other)
    {
        // Kích hoạt tự động khi player bước vào vùng trigger
        var pc = other.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        TryTeleport(pc);
    }

    void TryTeleport(PlayerController targetPlayer)
    {
        if (oneShot && _used) return;

        // Nếu có điều kiện state thì kiểm tra trước
        if (!string.IsNullOrEmpty(requiredFlagKey))
        {
            var ctx = GameBootstrap.Context;
            if (ctx == null || ctx.PlayerState == null)
            {
                Debug.LogWarning("[TeleportInteractable] PlayerState is not available.");
                return;
            }

            bool current = ctx.PlayerState.CheckFlag(requiredFlagKey);
            if (current != requiredValue)
            {
                Debug.Log("[TeleportInteractable] Condition not met: " + requiredFlagKey);
                return; // chưa qua được bảo vệ thì chưa cho vào office
            }
        }

        if (destinationPoint == null)
        {
            Debug.LogWarning("[TeleportInteractable] destinationPoint chưa được gán.");
            return;
        }

        // Ưu tiên player lấy từ tham số (trigger). Nếu null thì fallback về field được gán sẵn.
        if (targetPlayer == null)
        {
            targetPlayer = player;
        }

        if (targetPlayer == null)
        {
            Debug.LogWarning("[TeleportInteractable] PlayerController chưa được gán hoặc không tìm thấy từ trigger.");
            return;
        }

        _used = true;

        // Nếu có ScreenFader thì fade tối rồi teleport, sau đó sáng lại
        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeOutIn(1f, () =>
            {
                targetPlayer.TeleportTo(destinationPoint);
            });
        }
        else
        {
            // Không có fader thì teleport ngay lập tức
            targetPlayer.TeleportTo(destinationPoint);
        }
    }
}
