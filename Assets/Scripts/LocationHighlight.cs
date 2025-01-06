using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LocationHighlight : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private Image arrow;
    
    [Header("Outline Settings")]
    [SerializeField] private Color outlineColor = new Color(1f, 0.8f, 0f, 0.5f);
    [SerializeField] private Color pressedOutlineColor = new Color(1f, 0.4f, 0f, 0.5f);
    [SerializeField] private float outlineWidth = 1.05f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minBrightness = 0.4f;
    [SerializeField] private float maxBrightness = 0.8f;

    private static Shader outlineShader;
    private GameObject[] outlineObjects;
    private Material[] outlineMaterials;
    private bool isPressed;
    private float animationTime;
    private HashSet<MeshRenderer> processedRenderers;
    private bool isInitialized = false;
    private static int instanceCount = 0;
    private int instanceId;

    private void Awake()
    {
        instanceId = ++instanceCount;
        Debug.Log($"LocationHighlight {instanceId} - Awake Start");
        
        processedRenderers = new HashSet<MeshRenderer>();
        
        try
        {
            if (outlineShader == null)
            {
                outlineShader = Resources.Load<Shader>("Shaders/OutlineTransparent");
                Debug.Log($"LocationHighlight {instanceId} - Loading shader: {(outlineShader != null ? "Success" : "Failed")}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LocationHighlight {instanceId} - Error loading shader: {e}");
        }

        Debug.Log($"LocationHighlight {instanceId} - Awake Complete");
    }

    private void OnEnable()
    {
        Debug.Log($"LocationHighlight {instanceId} - OnEnable");
        if (!isInitialized)
        {
            InitializeOutlines();
        }
    }

    private void InitializeOutlines()
    {
        if (isInitialized) return;
        
        Debug.Log($"LocationHighlight {instanceId} - Initialize Start");
        
        try
        {
            if (outlineShader == null)
            {
                Debug.LogError($"LocationHighlight {instanceId} - Shader not found!");
                return;
            }

            processedRenderers.Clear();
            var renderers = GetComponentsInChildren<MeshRenderer>(true);
            Debug.Log($"LocationHighlight {instanceId} - Found {renderers.Length} renderers");

            // Create a list to store only valid renderers
            List<MeshRenderer> validRenderers = new List<MeshRenderer>();
            
            // Pre-filter valid renderers
            foreach (var renderer in renderers)
            {
                if (renderer != null && 
                    !renderer.gameObject.name.StartsWith("Outline_") && 
                    renderer.GetComponent<MeshFilter>()?.sharedMesh != null)
                {
                    validRenderers.Add(renderer);
                }
            }

            // Allocate arrays based on valid renderer count
            outlineObjects = new GameObject[validRenderers.Count];
            outlineMaterials = new Material[validRenderers.Count];

            // Create shared material to reduce memory usage
            Material sharedOutlineMaterial = new Material(outlineShader)
            {
                color = outlineColor
            };

            // Create outlines for valid renderers
            for (int i = 0; i < validRenderers.Count; i++)
            {
                if (!processedRenderers.Contains(validRenderers[i]))
                {
                    CreateOutline(validRenderers[i], i, sharedOutlineMaterial);
                }
            }

            if (arrow != null)
            {
                arrow.color = outlineColor;
            }

            isInitialized = true;
            Debug.Log($"LocationHighlight {instanceId} - Created {validRenderers.Count} outlines");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LocationHighlight {instanceId} - Error in Initialize: {e}");
        }
    }

    private void CreateOutline(MeshRenderer renderer, int index, Material sharedMaterial)
    {
        try
        {
            if (renderer == null || processedRenderers.Contains(renderer))
            {
                return;
            }

            processedRenderers.Add(renderer);

            string outlineName = $"Outline_{renderer.gameObject.name}_{instanceId}_{index}";
            GameObject outlineObj = new GameObject(outlineName);
            outlineObj.transform.SetParent(renderer.transform, false);
            outlineObj.transform.localPosition = Vector3.zero;
            outlineObj.transform.localRotation = Quaternion.identity;
            outlineObj.transform.localScale = Vector3.one * outlineWidth;

            MeshFilter outlineMeshFilter = outlineObj.AddComponent<MeshFilter>();
            outlineMeshFilter.sharedMesh = renderer.GetComponent<MeshFilter>().sharedMesh;

            MeshRenderer outlineRenderer = outlineObj.AddComponent<MeshRenderer>();
            outlineRenderer.sharedMaterial = sharedMaterial; // Use shared material
            outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            outlineRenderer.receiveShadows = false;
            outlineRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            outlineRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;

            outlineObjects[index] = outlineObj;
            outlineMaterials[index] = sharedMaterial;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LocationHighlight {instanceId} - Error creating outline: {e}");
        }
    }

    private void Update()
    {
        if (!isInitialized) return;
        
        try
        {
            animationTime += Time.deltaTime;
            UpdateOutlines();
            HandleTouchInput();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LocationHighlight {instanceId} - Error in Update: {e}");
        }
    }

    private void UpdateOutlines()
    {
        float pulseValue = (Mathf.Sin(animationTime * pulseSpeed) + 1f) * 0.5f;
        float currentBrightness = Mathf.Lerp(minBrightness, maxBrightness, pulseValue);

        Color baseColor = isPressed ? pressedOutlineColor : outlineColor;
        baseColor.a *= currentBrightness;

        // Since we're using a shared material, we only need to update it once
        if (outlineMaterials != null && outlineMaterials.Length > 0 && outlineMaterials[0] != null)
        {
            outlineMaterials[0].SetColor("_Color", baseColor);
        }

        if (arrow != null)
        {
            Color arrowColor = baseColor;
            arrowColor.a = 1f;
            arrow.color = arrowColor;
        }
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;
            bool isHitThis = false;

            if (Physics.Raycast(ray, out hit))
            {
                Transform hitTransform = hit.transform;
                int maxDepth = 20;
                int currentDepth = 0;

                while (hitTransform != null && currentDepth < maxDepth)
                {
                    if (hitTransform == transform)
                    {
                        isHitThis = true;
                        break;
                    }
                    hitTransform = hitTransform.parent;
                    currentDepth++;
                }
            }

            if (touch.phase == TouchPhase.Began && isHitThis)
            {
                SetPressed(true);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                SetPressed(false);
            }
        }
    }

    private void SetPressed(bool pressed)
    {
        if (isPressed == pressed) return;
        isPressed = pressed;
    }

    private void OnDestroy()
    {
        Debug.Log($"LocationHighlight {instanceId} - OnDestroy Start");
        
        try
        {
            if (outlineMaterials != null)
            {
                foreach (Material material in outlineMaterials)
                {
                    if (material != null)
                    {
                        Destroy(material);
                    }
                }
            }

            if (outlineObjects != null)
            {
                foreach (GameObject obj in outlineObjects)
                {
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            processedRenderers?.Clear();
            isInitialized = false;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LocationHighlight {instanceId} - Error in OnDestroy: {e}");
        }
        
        Debug.Log($"LocationHighlight {instanceId} - OnDestroy Complete");
    }
} 
