using System;
using UnityEngine;

namespace SeriousGame.App
{
    public class QuestService : MonoBehaviour
    {
        // Lưu trữ ID nhiệm vụ hiện tại
        public string CurrentObjectiveId;
        public string StartObjectiveId;

        [Header("Objective Toast")]
        [SerializeField] private ObjectiveToastMap objectiveToastMap;

        // Event để báo cho UI biết mà cập nhật text trên màn hình (dùng Action hoặc GameEventBus của bạn)
        public event Action<string> OnObjectiveChanged;

        // Hàm chuyển sang nhiệm vụ mới

        void Start()
        {
            // Khởi tạo nhiệm vụ đầu tiên khi bắt đầu game
            if (string.IsNullOrWhiteSpace(CurrentObjectiveId) && !string.IsNullOrWhiteSpace(StartObjectiveId))
                SetObjective(StartObjectiveId);
        }
        public void SetObjective(string objectiveId)
        {
            CurrentObjectiveId = objectiveId;
            Debug.Log($"[QuestService] Nhiệm vụ mới: {objectiveId}");
            
            // Báo cho UI (nếu có UI lắng nghe)
            OnObjectiveChanged?.Invoke(objectiveId);
            if (string.IsNullOrWhiteSpace(objectiveId)) return;

            if (objectiveToastMap != null && objectiveToastMap.TryGetMessage(objectiveId, out var message))
                GameEventBus.RaiseToastRequested(message);
            else
                GameEventBus.RaiseToastRequested(objectiveId);
        }
    }
}