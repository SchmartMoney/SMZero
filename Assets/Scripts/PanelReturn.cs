using UnityEngine;

namespace SMZero
{
    public class PanelReturn : MonoBehaviour
    {
        private void Awake()
        {
            // Ensure we have a RectTransform
            if (GetComponent<RectTransform>() == null)
            {
                Debug.LogError("[PanelReturn] Missing RectTransform component!");
            }
        }
    }
}
