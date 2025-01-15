using UnityEngine;

namespace SMZero
{
    public class GameBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            // Initialize managers
            var managers = GameManagers.Instance;
            Debug.Log("Game managers initialized");
        }
    }
} 