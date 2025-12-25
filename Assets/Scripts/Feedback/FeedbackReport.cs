using System.Collections.Generic;

namespace SeriousGame.Feedback
{
    public class FeedbackItem
    {
        public string title;
        public string detail;
        public int scoreDelta;
    }

    public class FeedbackReport
    {
        public int totalRiskScore;
        public List<FeedbackItem> items = new();
        public string rawSummaryText;
    }
}
