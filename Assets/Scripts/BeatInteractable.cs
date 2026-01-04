using UnityEngine;
using SeriousGame.Content;

namespace SeriousGame.Runtime
{
    public interface IInteractable
    {
        void Interact();
    }
    public class BeatInteractable : MonoBehaviour, IInteractable
    {
        // [Header("Gameplay")]

        // private bool _used = false;

        // [Header("NPC này giữ nhiệm vụ của Beat ID này:")]
        // public BeatSO beat;
        [Header("ID dùng để khớp với Beat")]
        public string interactId; // ví dụ: "guard"

    //     public void Interact()
    // {
    //     var currentBeat = EpisodeController.Instance.GetCurrentBeat(); // Bạn hãy viết thêm hàm lấy beat hiện tại
        
    //     if (currentBeat != null && currentBeat.beatId == beat.beatId)
    //     {
    //         EpisodeController.Instance.RunCurrentBeat();
    //     }
    //     else
    //     {
    //         Debug.Log("Chú bảo vệ: 'Cháu cứ đi làm việc của cháu đi' (Chưa tới lượt)");
    //     }
    // }
    public void Interact()
{   
        Debug.Log($"[BeatInteractable] Interact: {interactId}");
        EpisodeController.Instance.OnWorldInteract(interactId);
    }
    }
}
