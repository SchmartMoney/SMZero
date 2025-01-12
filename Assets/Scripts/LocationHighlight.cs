using UnityEngine;

public class LocationHighlight : MonoBehaviour
{
    private Material[] materials;
    private Color[] defaultColors;
    private bool[] canModifyColor;  // Track which materials can have their color modified
    private bool isInteractable = false;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeHighlight();
    }

    private bool HasColorProperty(Material material)
    {
        // Check for common color property names
        return material.HasProperty("_Color") || 
               material.HasProperty("_BaseColor") || 
               material.HasProperty("_MainColor") ||
               material.HasProperty("_EmissionColor");
    }

    private void SetMaterialColor(Material material, Color color)
    {
        if (material.HasProperty("_Color"))
            material.SetColor("_Color", color);
        else if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        else if (material.HasProperty("_MainColor"))
            material.SetColor("_MainColor", color);
        else if (material.HasProperty("_EmissionColor"))
            material.SetColor("_EmissionColor", color);
    }

    private Color GetMaterialColor(Material material)
    {
        if (material.HasProperty("_Color"))
            return material.GetColor("_Color");
        else if (material.HasProperty("_BaseColor"))
            return material.GetColor("_BaseColor");
        else if (material.HasProperty("_MainColor"))
            return material.GetColor("_MainColor");
        else if (material.HasProperty("_EmissionColor"))
            return material.GetColor("_EmissionColor");
        
        return Color.white; // Default color if no supported property found
    }

    private void InitializeHighlight()
    {
        if (isInitialized) return;

        // Get all renderers in this object and its children
        Renderer[] renderers = GetComponentsInChildren<Renderer>();        
        if (renderers != null && renderers.Length > 0)
        {
            // Initialize arrays to store materials and their default colors
            materials = new Material[renderers.Length];
            defaultColors = new Color[renderers.Length];
            canModifyColor = new bool[renderers.Length];

            // Store each renderer's material and default color
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                {
                    materials[i] = renderers[i].material;
                    if (materials[i] != null)
                    {
                        canModifyColor[i] = HasColorProperty(materials[i]);
                        if (canModifyColor[i])
                        {
                            defaultColors[i] = GetMaterialColor(materials[i]);
                        }
                    }
                }
            }
            isInitialized = true;
        }
    }

    public void SetHighlightColor(Color color)
    {
        if (!isInitialized)
        {
            InitializeHighlight();
        }

        
        if (materials != null && materials.Length > 0)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] != null && canModifyColor[i])
                {
                    SetMaterialColor(materials[i], color);
                    defaultColors[i] = color;
                }
            }
        }
    }

    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
    }

    private void OnMouseEnter()
    {
        if (!isInteractable || materials == null) return;
        
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null && canModifyColor[i])
            {
                SetMaterialColor(materials[i], Color.white); // Highlight color when mouse is over
            }
        }
    }

    private void OnMouseExit()
    {
        if (!isInteractable || materials == null) return;
        
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null && canModifyColor[i])
            {
                SetMaterialColor(materials[i], defaultColors[i]); // Return to default color
            }
        }
    }
} 
