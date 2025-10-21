using UnityEngine;

[System.Serializable]
public class FloorPlanBounds
{
    [Header("Real World Dimensions")]
    [Tooltip("Size of the academic block in meters (width, height)")]
    public Vector2 realWorldSize = new Vector2(50f, 30f);
    
    [Header("World Origin")]
    [Tooltip("Where (0,0) on floor plan maps to in world space")]
    public Vector3 worldOrigin = Vector3.zero;
    
    [Header("Floor Height")]
    [Tooltip("Height of the floor in world space")]
    public float floorHeight = 0f;
}

public class CoordinateConverter : MonoBehaviour
{
    [Header("Floor Plan Configuration")]
    [SerializeField] private FloorPlanBounds floorPlanBounds;
    [SerializeField] private RectTransform floorPlanRect;

    void Start()
    {
        AutoAssignFloorPlanRect();
    }

    void AutoAssignFloorPlanRect()
    {
        if (floorPlanRect == null)
        {
            // Find the floor plan RawImage
            UnityEngine.UI.RawImage floorPlan = FindFirstObjectByType<UnityEngine.UI.RawImage>();
            if (floorPlan != null)
            {
                floorPlanRect = floorPlan.rectTransform;
                Debug.Log("✅ CoordinateConverter: Auto-assigned floor plan RectTransform");
            }
            else
            {
                Debug.LogError("❌ CoordinateConverter: Could not find floor plan RawImage!");
            }
        }
    }

    public Vector3 UIToWorldPosition(Vector2 screenPosition)
    {
        if (floorPlanRect == null)
        {
            Debug.LogError("Floor plan RectTransform is not assigned!");
            return Vector3.zero;
        }
        
        // Convert screen position to local UI position
        Vector2 localPoint;
        bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            floorPlanRect, screenPosition, null, out localPoint);
        
        if (!isInside)
        {
            Debug.LogWarning("Touch point is outside floor plan area");
            return Vector3.zero;
        }
        
        // Normalize to 0-1 range
        Vector2 normalizedPos = new Vector2(
            (localPoint.x + floorPlanRect.rect.width * 0.5f) / floorPlanRect.rect.width,
            (localPoint.y + floorPlanRect.rect.height * 0.5f) / floorPlanRect.rect.height
        );
        
        // Clamp to ensure we stay within bounds
        normalizedPos.x = Mathf.Clamp01(normalizedPos.x);
        normalizedPos.y = Mathf.Clamp01(normalizedPos.y);
        
        // Convert to world coordinates
        Vector3 worldPos = new Vector3(
            floorPlanBounds.worldOrigin.x + (normalizedPos.x * floorPlanBounds.realWorldSize.x),
            floorPlanBounds.floorHeight,
            floorPlanBounds.worldOrigin.z + (normalizedPos.y * floorPlanBounds.realWorldSize.y)
        );

        Debug.Log($"🗺️ Coordinate Conversion: Screen {screenPosition} → Normalized ({normalizedPos.x:F3}, {normalizedPos.y:F3}) → World {worldPos}");
        Debug.Log($"   Building: Origin={floorPlanBounds.worldOrigin}, Size={floorPlanBounds.realWorldSize}");

        return worldPos;
    }
    
    public Vector2 WorldToUIPosition(Vector3 worldPosition)
    {
        // Convert world position back to normalized coordinates
        Vector2 normalizedPos = new Vector2(
            (worldPosition.x - floorPlanBounds.worldOrigin.x) / floorPlanBounds.realWorldSize.x,
            (worldPosition.z - floorPlanBounds.worldOrigin.z) / floorPlanBounds.realWorldSize.y
        );
        
        // Convert to local UI coordinates
        Vector2 localPoint = new Vector2(
            (normalizedPos.x - 0.5f) * floorPlanRect.rect.width,
            (normalizedPos.y - 0.5f) * floorPlanRect.rect.height
        );
        
        // Convert to screen position
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(null, 
            floorPlanRect.TransformPoint(localPoint));
        
        return screenPos;
    }
    
    public bool IsPositionValid(Vector2 screenPosition)
    {
        Vector2 localPoint;
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            floorPlanRect, screenPosition, null, out localPoint);
    }
    
    // Public getters for inspector viewing
    public FloorPlanBounds FloorPlanBounds => floorPlanBounds;
    public RectTransform FloorPlanRect => floorPlanRect;
    
    void OnValidate()
    {
        // Auto-assign in Start() instead, since CoordinateConverter is not a UI element
    }
}