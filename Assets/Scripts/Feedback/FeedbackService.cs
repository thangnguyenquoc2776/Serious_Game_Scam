using System.Collections.Generic;
using SeriousGame.Content;
using SeriousGame.Trace;

namespace SeriousGame.Feedback
{
    public class FeedbackService
    {
        private readonly TraceService _trace;
        private readonly TraceTaxonomySO _taxonomy;

        public FeedbackService(TraceService trace, TraceTaxonomySO taxonomy)
        {
            _trace = trace;
            _taxonomy = taxonomy;
        }

        public FeedbackReport GenerateEndChapterReport(string sessionId, EpisodeSO episode)
        {
            var report = new FeedbackReport();
            var events = _trace.GetSession(sessionId);

            // Count by traceTypeId
            var count = new Dictionary<string, int>();
            foreach (var e in events)
            {
                if (string.IsNullOrWhiteSpace(e.traceTypeId)) continue;
                if (!count.ContainsKey(e.traceTypeId)) count[e.traceTypeId] = 0;
                count[e.traceTypeId]++;
            }

            int score = 0;
            foreach (var kv in count)
            {
                var def = _taxonomy != null ? _taxonomy.Find(kv.Key) : null;
                int w = def != null ? def.riskWeight : 0;
                score += w * kv.Value;

                var title = def != null ? def.displayName : kv.Key;
                var detail = def != null ? def.description : "Không có mô tả traceType (bạn cần bổ sung TraceTypeSO).";

                report.items.Add(new FeedbackItem
                {
                    title = $"{title} (x{kv.Value})",
                    detail = detail,
                    scoreDelta = w * kv.Value
                });
            }

            report.totalRiskScore = score;

            // Compose text quick MVP
            var epTitle = episode != null ? episode.title : "Episode";
            var text = $"TỔNG KẾT: {epTitle}\n";
            text += $"RiskScore: {score} (âm là tốt, dương là rủi ro)\n\n";
            foreach (var it in report.items)
            {
                text += $"- {it.title}: {it.scoreDelta}\n  {it.detail}\n";
            }

            report.rawSummaryText = text;
            return report;
        }
    }
}
