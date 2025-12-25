using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/Content/Scene", fileName = "Scene")]
    public class SceneSO : ScriptableObject
    {
        public string sceneId = "S01";
        public string unitySceneName = "Episode_Demo";
        public BeatSO[] beats;
    }
}
