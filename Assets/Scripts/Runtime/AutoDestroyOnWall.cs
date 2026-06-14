using UnityEngine;

namespace SeriousGame.Runtime
{
    public class AutoDestroyOnWall : MonoBehaviour
    {
        [Header("Behavior")]
        [SerializeField] private bool deactivateInstead = true;

        public void HandleWallHit(bool? forceDeactivate = null)
        {
            var deactivate = forceDeactivate ?? deactivateInstead;
            if (deactivate)
                gameObject.SetActive(false);
            else
                Destroy(gameObject);
        }
    }
}
