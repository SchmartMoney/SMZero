using UnityEngine;

public class LocationHighlight : MonoBehaviour
{
    private Material[] materials;
    private Color[] defaultColors;
    private bool isInteractable = false;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeHighlight();
    }

    private void InitializeHighlight()
    {
        if (isInitialized) return;

        // Get all renderers in this object and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Debug.Log($"[LocationHighlight] {gameObject.name}: Found {renderers?.Length ?? 0} renderers");
        
        if (renderers != null && renderers.Length > 0)
        {
            // Initialize arrays to store materials and their default colors
            materials = new Material[renderers.Length];
            defaultColors = new Color[renderers.Length];

            // Store each renderer's material and default color
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    materials[i] = renderers[i].material;
                    if (materials[i] != null)
                    {
                        defaultColors[i] = materials[i].color;
                        Debug.Log($"[LocationHighlight] {gameObject.name}: Initialized material {i} with color {defaultColors[i]}");
                    }
                }
            }
            isInitialized = true;
        }
        else
        {
            Debug.LogError($"LocationHighlight on {gameObject.name} needs a Renderer component in itself or its children!");
        }
    }

    public void SetHighlightColor(Color color)
    {
        if (!isInitialized)
        {
            InitializeHighlight();
        }

        Debug.Log($"[LocationHighlight] {gameObject.name}: Setting highlight color to {color}");
        
        if (materials != null && materials.Length > 0)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    materials[i].color = color;
                    defaultColors[i] = color;
                }
            }
        }
        else
        {
            Debug.LogError($"[LocationHighlight] {gameObject.name}: No materials found to set color!");
        }
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        Debug.Log($"[LocationHighlight] {gameObject.name}: Set interactable to {interactable}");
    }

    private void OnMouseEnter()
    {
        Debug.Log($"[LocationHighlight] {gameObject.name}: OnMouseEnter (isInteractable: {isInteractable})");
        if (!isInteractable || materials == null) return;
        
        foreach (Material material in materials)
        {
            if (material != null)
            {
                material.color = Color.white; // Highlight color when mouse is over
            }
        }
    }

    private void OnMouseExit()
    {
        Debug.Log($"[LocationHighlight] {gameObject.name}: OnMouseExit (isInteractable: {isInteractable})");
        if (!isInteractable || materials == null) return;
        
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                materials[i].color = defaultColors[i]; // Return to default color
            }
        }
    }
} 
