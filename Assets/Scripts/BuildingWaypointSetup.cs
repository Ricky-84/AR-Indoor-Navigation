using UnityEngine;

/// <summary>
/// Quick setup script to create all waypoints for your building
/// Attach to any GameObject in SampleScene and use context menu
/// </summary>
public class BuildingWaypointSetup : MonoBehaviour
{
    [Header("Building Configuration")]
    [Tooltip("Total width of building in meters")]
    public float buildingWidth = 120f;

    [Tooltip("Total depth of building in meters")]
    public float buildingDepth = 80f;

    [Tooltip("Floor height (Y coordinate)")]
    public float floorHeight = 0f;

    [Header("Waypoint Prefab")]
    [Tooltip("Drag the cylinder/cube you want to use for waypoints")]
    public GameObject waypointPrefab;

    /// <summary>
    /// Create all waypoints based on your floor plan
    /// Adjust positions after creation based on your actual building layout
    /// </summary>
    [ContextMenu("Create All Building Waypoints")]
    public void CreateAllWaypoints()
    {
        Debug.Log("🏢 Creating building waypoints...");

        // Calculate center and offsets
        float centerX = 0f;
        float centerZ = 0f;
        float leftX = centerX - buildingWidth * 0.4f;
        float rightX = centerX + buildingWidth * 0.4f;
        float frontZ = centerZ - buildingDepth * 0.3f;
        float midZ = centerZ;
        float backZ = centerZ + buildingDepth * 0.3f;

        // LEFT SIDE BLOCKS (F, E, D)
        CreateWaypoint("F Block", new Vector3(leftX, floorHeight, backZ), Color.blue, false, false);
        CreateWaypoint("E Block", new Vector3(leftX, floorHeight, midZ), Color.blue, false, false);
        CreateWaypoint("D Block", new Vector3(leftX, floorHeight, frontZ), Color.blue, false, false);

        // RIGHT SIDE BLOCKS (A, B, C)
        CreateWaypoint("C Block", new Vector3(rightX, floorHeight, backZ), Color.blue, false, false);
        CreateWaypoint("B Block", new Vector3(rightX, floorHeight, midZ), Color.blue, false, false);
        CreateWaypoint("A Block", new Vector3(rightX, floorHeight, frontZ), Color.blue, false, false);

        // CENTER BLOCKS
        CreateWaypoint("West Block", new Vector3(centerX - 20, floorHeight, midZ), Color.blue, false, false);
        CreateWaypoint("Center Block", new Vector3(centerX, floorHeight, midZ), Color.blue, false, false);
        CreateWaypoint("East Block", new Vector3(centerX + 20, floorHeight, midZ), Color.blue, false, false);

        // ENTRANCES/EXITS
        CreateWaypoint("Main Entrance", new Vector3(centerX, floorHeight, frontZ - 10), Color.green, true, false);
        CreateWaypoint("Side Entrance", new Vector3(rightX + 10, floorHeight, frontZ), Color.green, true, false);
        CreateWaypoint("Emergency Exit", new Vector3(centerX, floorHeight, backZ + 10), Color.red, false, true);

        Debug.Log("✅ Created all waypoints! Adjust positions in Scene view as needed.");
        Debug.Log("💡 TIP: Use the Move tool (W key) to reposition waypoints to match your building.");
    }

    /// <summary>
    /// Create a single waypoint at specified position
    /// </summary>
    void CreateWaypoint(string name, Vector3 position, Color color, bool isEntrance, bool isExit)
    {
        GameObject waypoint;

        if (waypointPrefab != null)
        {
            // Use custom prefab
            waypoint = Instantiate(waypointPrefab, position, Quaternion.identity);
            waypoint.name = $"Waypoint_{name}";
        }
        else
        {
            // Create default cylinder
            waypoint = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            waypoint.name = $"Waypoint_{name}";
            waypoint.transform.position = position;
            waypoint.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        }

        // Add Waypoint3D component
        Waypoint3D waypointComponent = waypoint.AddComponent<Waypoint3D>();
        waypointComponent.waypointName = name;
        waypointComponent.isEntrance = isEntrance;
        waypointComponent.isExit = isExit;
        waypointComponent.waypointColor = color;

        // Set visual color
        MeshRenderer renderer = waypoint.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            renderer.material = mat;
        }

        Debug.Log($"📍 Created: {name} at {position}");
    }

    /// <summary>
    /// Clear all waypoints (useful for starting over)
    /// </summary>
    [ContextMenu("Clear All Waypoints")]
    public void ClearAllWaypoints()
    {
        Waypoint3D[] waypoints = FindObjectsByType<Waypoint3D>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var waypoint in waypoints)
        {
            DestroyImmediate(waypoint.gameObject);
        }

        Debug.Log($"🧹 Cleared {waypoints.Length} waypoints");
    }

    /// <summary>
    /// Show current waypoint count
    /// </summary>
    [ContextMenu("Count Waypoints")]
    public void CountWaypoints()
    {
        Waypoint3D[] waypoints = FindObjectsByType<Waypoint3D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"📊 Total waypoints in scene: {waypoints.Length}");

        foreach (var wp in waypoints)
        {
            Debug.Log($"   • {wp.waypointName} at {wp.transform.position}");
        }
    }
}
