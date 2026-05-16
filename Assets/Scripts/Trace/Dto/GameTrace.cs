using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeriousGame.Trace
{
    [Serializable]
    public class GameTrace
    {
        [JsonIgnore]
        public string sessionId;

        public string milestone_id;
        public string route_id;
        public string choice_id; // 1 2 3
        public string trace_id; // cái này giống như verb /action trong xapi
        public string object_name;
        // public string action_id;
        // public string object_id; //node name
        public long timestamp;
        public Dictionary<string, int> score_state = new Dictionary<string, int>();
    }
}
