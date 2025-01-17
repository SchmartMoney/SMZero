using UnityEngine;
using System.Collections;

namespace SMZero
{
    public class ZoneSlotsManager : MonoBehaviour
    {
        private RectTransform rectTransform;
        private PanelSlot targetPanel;
        private bool isAttached = false;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            StartCoroutine(WaitForHUDAndAttach());
        }

        private IEnumerator WaitForHUDAndAttach()
        {
            // Wait until we find the HUD
            while (!isAttached)
            {
                targetPanel = FindObjectOfType<PanelSlot>();
                if (targetPanel != null)
                {
                    AttachToPanel();
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void AttachToPanel()
        {
            if (targetPanel == null) return;

            // Make this object a child of the panel
            transform.SetParent(targetPanel.transform, false);
            
            // Reset position and scale
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            
            isAttached = true;
            Debug.Log("[ZoneSlotsManager] Successfully attached to panel");
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
