using UnityEngine;
using UnityEngine.UI;

public class ImageAspectFixer : MonoBehaviour
{
    void Start()
    {
        FixFloorPlanAspectRatio();
    }
    
    [ContextMenu("Fix Floor Plan Aspect Ratio")]
    public void FixFloorPlanAspectRatio()
    {
        RawImage floorPlanImage = FindFirstObjectByType<RawImage>();
        if (floorPlanImage != null && floorPlanImage.texture != null)
        {
            Debug.Log("🔧 Fixing floor plan aspect ratio...");
            
            // Get the texture dimensions
            float textureWidth = floorPlanImage.texture.width;
            float textureHeight = floorPlanImage.texture.height;
            float aspectRatio = textureWidth / textureHeight;
            
            Debug.Log($"📐 Original texture: {textureWidth} x {textureHeight} (aspect: {aspectRatio:F2})");
            
            // Get current RectTransform
            RectTransform rectTransform = floorPlanImage.rectTransform;
            
            // Calculate proper size while maintaining aspect ratio
            float currentWidth = rectTransform.rect.width;
            float currentHeight = rectTransform.rect.height;
            
            // Fit to width or height, whichever is smaller
            float newWidth, newHeight;
            
            if (aspectRatio > (currentWidth / currentHeight))
            {
                // Image is wider than container - fit to width
                newWidth = currentWidth;
                newHeight = currentWidth / aspectRatio;
            }
            else
            {
                // Image is taller than container - fit to height
                newHeight = currentHeight;
                newWidth = currentHeight * aspectRatio;
            }
            
            // Apply new size
            rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            
            Debug.Log($"✅ Fixed aspect ratio: {newWidth:F0} x {newHeight:F0}");
            Debug.Log($"📏 Aspect ratio preserved: {(newWidth/newHeight):F2}");
            
            // Update coordinate system to match new dimensions
            UpdateCoordinateSystemForNewSize(newWidth, newHeight);
        }
        else
        {
            Debug.LogWarning("⚠️ No floor plan image found to fix aspect ratio");
        }
    }
    
    void UpdateCoordinateSystemForNewSize(float width, float height)
    {
        CoordinateConverter coordinator = FindFirstObjectByType<CoordinateConverter>();
        if (coordinator != null)
        {
            // Update the floorPlanRect reference
            RawImage floorPlanImage = FindFirstObjectByType<RawImage>();
            var rectField = coordinator.GetType().GetField("floorPlanRect", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (rectField != null)
            {
                rectField.SetValue(coordinator, floorPlanImage.rectTransform);
                Debug.Log("✅ Updated CoordinateConverter with new dimensions");
            }
        }
    }
}