using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeriousGame.App
{
    public class SceneService
    {
        public void Load(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return;
            SceneManager.LoadScene(sceneName);
        }

        public AsyncOperation LoadAsync(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return null;
            return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
