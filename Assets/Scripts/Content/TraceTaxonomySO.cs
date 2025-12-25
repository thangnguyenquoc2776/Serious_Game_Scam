// Assets/Scripts/Content/TraceTaxonomySO.cs
using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Trace/TraceTaxonomy", fileName = "TraceTaxonomy")]
    public class TraceTaxonomySO : ScriptableObject
    {
        public TraceTypeSO[] traceTypes;

        public TraceTypeSO Find(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || traceTypes == null) return null;

            foreach (var t in traceTypes)
                if (t != null && t.traceTypeId == id) return t;

            return null;
        }
    }
}
