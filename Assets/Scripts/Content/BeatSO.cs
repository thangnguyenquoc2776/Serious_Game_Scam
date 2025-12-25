using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Content/Beat", fileName = "Beat")]
    public class BeatSO : ScriptableObject
    {
        public string beatId = "B01";
        [TextArea] public string objectiveText;

        [Header("Core")]
        public InteractionSO interaction; // MVP quick
        public InteractionSO[] interactions; // future extension

        [Header("Flow")]
        public bool endChapter;

        public InteractionSO GetPrimaryInteraction()
        {
            if (interactions != null && interactions.Length > 0 && interactions[0] != null)
                return interactions[0];
            return interaction;
        }
    }
}
