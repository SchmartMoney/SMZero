using UnityEngine;

public class LocationHighlight : MonoBehaviour
{
    private Material[] materials;
    private Color[] defaultColors;
    private bool isInteractable = false;

    private void Awake()
    {
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
                materials[i] = renderers[i].material;
                defaultColors[i] = materials[i].color;
                Debug.Log($"[LocationHighlight] {gameObject.name}: Renderer[{i}] = {renderers[i].gameObject.name}, Material = {materials[i].name}");
            }
        }
        else
        {
            Debug.LogError($"LocationHighlight on {gameObject.name} needs a Renderer component in itself or its children!");
        }
    }

    public void SetHighlightColor(Color color)
    {
        if (materials != null)
        {
            Debug.Log($"[LocationHighlight] {gameObject.name}: Setting highlight color to {color}");
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null)
                {
                    materials[i].color = color;
                    defaultColors[i] = color;
                }
            }
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
