using UnityEngine;
using UnityEngine.UI;

public class ImmediateSizeFix : MonoBehaviour
{
    void Update()
    {
        // Press SPACE to apply immediate size fix
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("🚀 SPACE pressed - applying immediate floor plan size fix!");
            ApplyImmediateFix();
        }
    }
    
    void ApplyImmediateFix()
    {
        Debug.Log("🔧 Applying manual floor plan size fix...");
        ApplyManualFix();
    }
    
    void ApplyManualFix()
    {
        RawImage floorPlan = FindFirstObjectByType<RawImage>();
        if (floorPlan != null)
        {
            Debug.Log("🔧 Applying manual size fix...");
            
            // Make it much smaller
            RectTransform rect = floorPlan.rectTransform;
            rect.sizeDelta = new Vector2(500, 300); // Fixed reasonable size
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, 20);
            
            Debug.Log("✅ Manual fix applied - image should be much smaller now");
        }
    }
}