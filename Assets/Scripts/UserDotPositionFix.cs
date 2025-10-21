using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick fix for user location dot positioning issue
/// This ensures the red dot appears where you click
/// </summary>
public class UserDotPositionFix : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject userLocationDot;
    [SerializeField] private RawImage floorPlanImage;
    [SerializeField] private Canvas parentCanvas;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private Vector2 lastClickPosition;

    void Start()
    {
        // Auto-find if not assigned
        if (parentCanvas == null)
            parentCanvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        // Test positioning with mouse clicks
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = Input.mousePosition;
            lastClickPosition = clickPos;

            if (showDebugGizmos)
            {
                Debug.Log($"🖱️ Click at screen position: {clickPos}");
                Debug.Log($"   Screen size: {Screen.width}x{Screen.height}");

                if (userLocationDot != null)
                {
                    RectTransform dotRect = userLocationDot.GetComponent<RectTransform>();
                    Debug.Log($"   Dot RectTransform position: {dotRect.position}");
                    Debug.Log($"   Dot anchored position: {dotRect.anchoredPosition}");
                }

                if (floorPlanImage != null)
                {
                    RectTransform imageRect = floorPlanImage.rectTransform;

                    // Check if click is within floor plan bounds
                    Vector2 localPoint;
                    bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        imageRect, clickPos, parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera, out localPoint);

                    Debug.Log($"   Click is {(isInside ? "INSIDE" : "OUTSIDE")} floor plan");
                    Debug.Log($"   Local point in floor plan: {localPoint}");
                    Debug.Log($"   Floor plan rect size: {imageRect.rect.size}");
                    Debug.Log($"   Floor plan position: {imageRect.position}");
                }
            }
        }
    }

    /// <summary>
    /// Correct way to position UI element at screen position
    /// </summary>
    public void PositionDotAtScreenPosition(Vector2 screenPosition)
    {
        if (userLocationDot == null)
        {
            Debug.LogError("User location dot is null!");
            return;
        }

        RectTransform dotRect = userLocationDot.GetComponent<RectTransform>();
        if (dotRect == null) return;

        if (parentCanvas == null)
        {
            Debug.LogError("Parent canvas is null!");
            return;
        }

        // For Screen Space - Overlay canvas, just use screen position directly
        if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            dotRect.position = screenPosition;
            Debug.Log($"✅ Positioned dot at screen position: {screenPosition}");
        }
        // For Screen Space - Camera or World Space
        else
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.GetComponent<RectTransform>(),
                screenPosition,
                parentCanvas.worldCamera,
                out localPoint);

            dotRect.localPosition = localPoint;
            Debug.Log($"✅ Positioned dot at local position: {localPoint}");
        }
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos || userLocationDot == null) return;

        // Draw debug sphere at dot position in scene view
        if (userLocationDot.activeInHierarchy)
        {
            RectTransform dotRect = userLocationDot.GetComponent<RectTransform>();
            if (dotRect != null)
            {
                Vector3 worldPos = dotRect.position;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(worldPos, 20f);
            }
        }
    }
}
