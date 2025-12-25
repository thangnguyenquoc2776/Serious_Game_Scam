// Assets/Scripts/Content/TraceTypeSO.cs
using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Trace/TraceType", fileName = "TraceType")]
    public class TraceTypeSO : ScriptableObject
    {
        [Header("Identity")]
        public string traceTypeId = "T_VERIFY_OFFICIAL";

        [Header("UI")]
        public string displayName = "Xác minh kênh chính thống";
        [TextArea] public string description;

        [Header("Scoring")]
        // riskWeight > 0: hành vi rủi ro
        // riskWeight < 0: hành vi tốt
        public int riskWeight = -2;
    }
}
