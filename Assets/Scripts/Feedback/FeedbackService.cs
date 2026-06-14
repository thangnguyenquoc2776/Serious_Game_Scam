using System.Collections.Generic;
using SeriousGame.Trace;
using SeriousGame.State;
using SeriousGame.Content;

namespace SeriousGame.Feedback
{
    public class FeedbackService
    {
        private readonly TraceService _trace;
        private readonly EvidenceMappingSO _mapping;
        private readonly PlayerStateService _state;

        public FeedbackService(
            TraceService trace,
            EvidenceMappingSO mapping,
            PlayerStateService state = null)
        {
            _trace = trace;
            _mapping = mapping;
            _state = state;
        }

        public void ApplyTrace(string traceId)
        {
            if (_state == null) return;
            if (_mapping == null || string.IsNullOrWhiteSpace(traceId)) return;

            var entry = _mapping.Find(traceId);
            if (entry == null || entry.scoreDeltas == null) return;

            for (int i = 0; i < entry.scoreDeltas.Length; i++)
            {
                var delta = entry.scoreDeltas[i];
                if (delta == null || string.IsNullOrWhiteSpace(delta.scoreKey)) continue;
                if (!GameStateKeys.IsValid(delta.scoreKey)) continue;
                _state.Add(delta.scoreKey, delta.delta);
            }
        }

        // Quick and simple feedback report generation based on trace events and player state.ss
        public FeedbackReport GenerateEndChapterReport(string sessionId) // cái này hiện tại đang tính cộng trừ cơ bản, chưa có logic phức tạp gì, nhưng sau này có thể mở rộng thêm các kiểu phân tích khác (ví dụ: dựa trên sequence của trace events, hoặc kết hợp với state snapshot để đưa ra nhận định chính xác hơn)
        {
            var report = new FeedbackReport();
            var events = _trace.GetSession(sessionId);

            // Count by trace_id
            var count = new Dictionary<string, int>();
            foreach (var e in events)
            {
                if (string.IsNullOrWhiteSpace(e.trace_id)) continue;
                if (!count.ContainsKey(e.trace_id)) count[e.trace_id] = 0;
                count[e.trace_id]++;
            }

            int score = 0;
            foreach (var kv in count)
            {
                var entry = _mapping != null ? _mapping.Find(kv.Key) : null;
                int deltaSum = GetTotalDelta(entry);
                score += deltaSum * kv.Value;

                var title = entry != null && !string.IsNullOrWhiteSpace(entry.displayName) ? entry.displayName : kv.Key;
                var detail = entry != null && !string.IsNullOrWhiteSpace(entry.description)
                    ? entry.description
                    : "Không có mô tả evidence mapping (bạn cần bổ sung EvidenceMappingSO).";

                report.items.Add(new FeedbackItem
                {
                    title = $"{title} (x{kv.Value})",
                    detail = detail,
                    scoreDelta = deltaSum * kv.Value
                });
            }

            report.totalRiskScore = score;

            // Compose text quick MVP
            var text = "TỔNG KẾT\n";
            text += $"RiskScore: {score} (âm là tốt, dương là rủi ro)\n\n";
            foreach (var it in report.items)
            {
                text += $"- {it.title}: {it.scoreDelta}\n  {it.detail}\n";
            }

            if (_state != null)
            {
                var snapshot = _state.GetSnapshot();
                if (snapshot != null && snapshot.entries != null && snapshot.entries.Count > 0)
                {
                    text += "\nState snapshot:\n";
                    for (int i = 0; i < snapshot.entries.Count; i++)
                    {
                        var entry = snapshot.entries[i];
                        if (entry == null) continue;
                        text += $"- {entry.key}: {entry.value}\n";
                    }
                }
            }

            report.rawSummaryText = text;
            return report;
        }

        private static int GetTotalDelta(EvidenceMappingEntry entry)
        {
            if (entry == null || entry.scoreDeltas == null) return 0;

            int sum = 0;
            for (int i = 0; i < entry.scoreDeltas.Length; i++)
            {
                var delta = entry.scoreDeltas[i];
                if (delta == null) continue;
                sum += delta.delta;
            }

            return sum;
        }
    }
}
