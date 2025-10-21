using UnityEngine;
using WrightAngle.Waypoint;

/// <summary>
/// Bridges the 2D floor plan system with the AR waypoint system
/// Place this in SampleScene (AR scene)
/// </summary>
public class ARNavigationBridge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform arCamera;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;

    private WaypointTarget destinationTarget;
    private GameObject userStartMarker;

    void Start()
    {
        // Wait for AR to initialize
        Invoke("SetupNavigation", 1f);
    }

    void SetupNavigation()
    {
        if (!NavigationData.hasValidData)
        {
            Debug.LogWarning("⚠️ No navigation data from floor plan scene!");
            return;
        }

        if (showDebugInfo)
        {
            Debug.Log("🎯 AR Navigation Bridge - Setting up...");
            NavigationData.LogData();
        }

        // Validate navigation data
        if (NavigationData.userStartPosition == Vector3.zero)
        {
            Debug.LogError("❌ Invalid start position!");
            return;
        }

        if (NavigationData.destinationPosition == Vector3.zero)
        {
            Debug.LogError("❌ Invalid destination position!");
            return;
        }

        if (string.IsNullOrEmpty(NavigationData.destinationName))
        {
            Debug.LogError("❌ Invalid destination name!");
            return;
        }

        try
        {
            // Create visual marker at user's start position
            CreateUserStartMarker();

            // Find or create destination waypoint target
            SetupDestinationTarget();

            // Position AR camera/XR Origin at user's start position
            PositionCameraAtStartLocation();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error setting up navigation: {e.Message}\n{e.StackTrace}");
        }
    }

    void CreateUserStartMarker()
    {
        // Apply the same coordinate flip that DirectFloorPlanLoader uses for destinations
        Vector3 flippedStartPos = FlipCoordinates(NavigationData.userStartPosition);

        userStartMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        userStartMarker.name = "UserStartPosition";
        userStartMarker.transform.position = flippedStartPos;
        userStartMarker.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);

        // Make it green
        MeshRenderer renderer = userStartMarker.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = Color.green;
            renderer.material = mat;
        }

        if (showDebugInfo)
        {
            Debug.Log($"✅ Created start marker at: {flippedStartPos} (original: {NavigationData.userStartPosition})");
        }
    }

    void SetupDestinationTarget()
    {
        Vector3 destPos = NavigationData.destinationPosition;
        string destName = NavigationData.destinationName;

        // Try to find existing waypoint target near destination
        WaypointTarget[] allTargets = FindObjectsByType<WaypointTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (showDebugInfo)
        {
            Debug.Log($"🔍 Found {allTargets.Length} existing waypoint targets in scene");
        }

        if (allTargets != null && allTargets.Length > 0)
        {
            foreach (var target in allTargets)
            {
                if (target == null) continue;

                float distance = Vector3.Distance(target.transform.position, destPos);
                if (distance < 2f) // Within 2 meters
                {
                    destinationTarget = target;
                    target.DisplayName = destName;
                    target.ActivateWaypoint();

                    if (showDebugInfo)
                    {
                        Debug.Log($"✅ Found existing waypoint target: {destName} at distance {distance:F2}m");
                    }
                    return;
                }
            }
        }

        // Create new waypoint target if none found
        GameObject targetObj = new GameObject($"Destination_{destName}");
        targetObj.transform.position = destPos;

        destinationTarget = targetObj.AddComponent<WaypointTarget>();
        destinationTarget.DisplayName = destName;
        destinationTarget.ActivateWaypoint();

        // Add visual marker
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        marker.transform.SetParent(targetObj.transform);
        marker.transform.localPosition = Vector3.zero;
        marker.transform.localScale = new Vector3(0.5f, 0.05f, 0.5f);

        MeshRenderer renderer = marker.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = Color.red;
            renderer.material = mat;
        }

        if (showDebugInfo)
        {
            Debug.Log($"✅ Created new waypoint target: {destName} at {destPos}");
        }
    }

    void PositionCameraAtStartLocation()
    {
        // Apply the same coordinate flip
        Vector3 flippedStartPos = FlipCoordinates(NavigationData.userStartPosition);

        // Add height offset for eye level (1.6m average person eye height)
        flippedStartPos.y = 1.6f;

        // Find XR Origin (AR camera parent)
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
        if (xrOrigin == null)
        {
            xrOrigin = GameObject.Find("XR Origin");
        }

        if (xrOrigin != null)
        {
            // Move XR Origin to user's start position at eye height
            xrOrigin.transform.position = flippedStartPos;

            if (showDebugInfo)
            {
                Debug.Log($"✅ Moved XR Origin to start position: {flippedStartPos} (original: {NavigationData.userStartPosition})");
                Debug.Log($"   Setting Y=1.6m for eye height (was Y=0 ground level)");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ XR Origin not found - using default camera position");

            // Fallback: try to move main camera directly
            if (arCamera != null)
            {
                arCamera.position = flippedStartPos;

                if (showDebugInfo)
                {
                    Debug.Log($"✅ Moved AR Camera to start position: {flippedStartPos}");
                }
            }
        }
    }

    /// <summary>
    /// Flips coordinates to match floor plan orientation (same logic as DirectFloorPlanLoader)
    /// Building bounds: 300m x 140m, origin at (-150.80, 0, -71.48)
    /// </summary>
    Vector3 FlipCoordinates(Vector3 worldPos)
    {
        // Building configuration from WaypointManager3D
        Vector3 buildingOrigin = new Vector3(-150.80000305f, 0f, -71.4750004f);
        Vector2 buildingSize = new Vector2(300f, 140f);

        // Convert world position to normalized (0-1)
        float normalizedX = (worldPos.x - buildingOrigin.x) / buildingSize.x;
        float normalizedZ = (worldPos.z - buildingOrigin.z) / buildingSize.y;

        // Flip both coordinates (1 - normalized)
        float flippedX = 1f - normalizedX;
        float flippedZ = 1f - normalizedZ;

        // Convert back to world position
        Vector3 flipped = new Vector3(
            buildingOrigin.x + (flippedX * buildingSize.x),
            worldPos.y,
            buildingOrigin.z + (flippedZ * buildingSize.y)
        );

        return flipped;
    }

    void OnDestroy()
    {
        // Clean up navigation data when leaving AR scene
        NavigationData.Reset();
    }
}
