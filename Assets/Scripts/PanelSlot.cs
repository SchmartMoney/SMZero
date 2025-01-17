using UnityEngine;

namespace SMZero
{
    public class PanelSlot : MonoBehaviour
    {
        private void Awake()
        {
            // Ensure we have a RectTransform
            if (GetComponent<RectTransform>() == null)
            {
                Debug.LogError("[PanelSlot] Missing RectTransform component!");
            }
        }
    }
}
