using UnityEngine;

namespace SeriousGame.Content
{
    [CreateAssetMenu(menuName = "SeriousGame/AppConfig", fileName = "AppConfig")]
    public class AppConfigSO : ScriptableObject
    {
        [Header("Scene Names")]
        public string bootSceneName = "Boot";
        public string mainMenuSceneName = "MainMenu";
        public string demoEpisodeSceneName = "Episode_Demo";

        [Header("Flow")]
        public bool skipMainMenuAndAutoStartEpisode = true;

        [Header("Content")]
        public TraceTaxonomySO traceTaxonomy;
    }
}
