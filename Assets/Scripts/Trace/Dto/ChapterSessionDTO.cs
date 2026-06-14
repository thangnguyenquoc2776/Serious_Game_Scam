using System;
using System.Collections.Generic;

namespace SeriousGame.Trace
{
    [Serializable]
    public class ChapterSessionDTO
    {
        // [Serializable]
        // public class FinalScoresData
        // {
        //     public int SCORE_HELP_SEEKING;
        //     public int SCORE_PRESSURE_RESISTANCE;
        //     public int SCORE_INFORMATION_VERIFICATION;
        //     public int SCORE_RISK_RECOGNITION;
        //     public int SCORE_COMMUNITY_WARNING;
        // }

        // [Serializable]
        // public class TraceData
        // {
        //     public string milestone_id;
        //     public string route_id;
        //     public string choice_id;
        //     public string trace_id;
        //     public string action_id;
        //     public string object_id;
        //     public long timestamp;
        //     public Dictionary<string, int> score_state = new Dictionary<string, int>();
        // }

        public string session_id;
        public string player_id;
        public long start_time;
        public long end_time;
        public float total_playtime_seconds;
        public bool is_completed;
        public FinalScores final_scores;
        public List<GameTrace> traces = new List<GameTrace>();
    }
}
