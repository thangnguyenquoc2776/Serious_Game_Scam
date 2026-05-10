using System;
using System.Collections.Generic;

namespace SeriousGame.Trace
{
    [Serializable]
    public class GameTrace
    {
        public string milestone_id;
        public string route_id;
        public string choice_id;
        public string trace_id;
        public string action_id;
        public string object_id;
        public long timestamp;
        public Dictionary<string, int> score_state = new Dictionary<string, int>();
    }
}
