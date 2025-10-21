using UnityEngine;

/// <summary>
/// Helper script to create waypoint models easily in your 3D scene
/// </summary>
public class WaypointCreator : MonoBehaviour
{
    [Header("Waypoint Creation Settings")]
    public string waypointBaseName = "Waypoint";
    public WaypointType defaultType = WaypointType.Normal;
    public bool autoNumberWaypoints = true;
    
    [Header("Visual Settings")]
    public PrimitiveType waypointShape = PrimitiveType.Cylinder;
    public Vector3 waypointScale = new Vector3(0.5f, 0.1f, 0.5f);
    public Material waypointMaterial;
    
    private static int waypointCounter = 1;
    
    public enum WaypointType
    {
        Normal,
        Entrance,
        Exit
    }
    
    /// <summary>
    /// Create waypoint at current position
    /// </summary>
    [ContextMenu("Create Waypoint Here")]
    public void CreateWaypointHere()
    {
        CreateWaypointAt(transform.position, defaultType);
    }
    
    /// <summary>
    /// Create entrance waypoint
    /// </summary>
    [ContextMenu("Create Entrance Waypoint")]
    public void CreateEntranceWaypoint()
    {
        CreateWaypointAt(transform.position, WaypointType.Entrance);
    }
    
    /// <summary>
    /// Create exit waypoint
    /// </summary>
    [ContextMenu("Create Exit Waypoint")]
    public void CreateExitWaypoint()
    {
        CreateWaypointAt(transform.position, WaypointType.Exit);
    }
    
    /// <summary>
    /// Create multiple waypoints for common building areas
    /// </summary>
    [ContextMenu("Create Building Waypoints Template")]
    public void CreateBuildingWaypointsTemplate()
    {
        Vector3 basePos = transform.position;
        
        // Create entrance waypoints
        CreateWaypointAt(basePos + new Vector3(-50, 0, -30), WaypointType.Entrance, "Main Entrance");
        CreateWaypointAt(basePos + new Vector3(50, 0, -30), WaypointType.Entrance, "Side Entrance");
        
        // Create block waypoints based on your floor plan
        CreateWaypointAt(basePos + new Vector3(-40, 0, 20), WaypointType.Normal, "F Block");
        CreateWaypointAt(basePos + new Vector3(-40, 0, 0), WaypointType.Normal, "E Block");
        CreateWaypointAt(basePos + new Vector3(-40, 0, -20), WaypointType.Normal, "D Block");
        
        CreateWaypointAt(basePos + new Vector3(40, 0, 20), WaypointType.Normal, "C Block");
        CreateWaypointAt(basePos + new Vector3(40, 0, 0), WaypointType.Normal, "B Block");
        CreateWaypointAt(basePos + new Vector3(40, 0, -20), WaypointType.Normal, "A Block");
        
        CreateWaypointAt(basePos + new Vector3(-20, 0, 0), WaypointType.Normal, "West Block");
        CreateWaypointAt(basePos + new Vector3(0, 0, 0), WaypointType.Normal, "Center Block");
        CreateWaypointAt(basePos + new Vector3(20, 0, 0), WaypointType.Normal, "East Block");
        
        // Create exit waypoints
        CreateWaypointAt(basePos + new Vector3(0, 0, 30), WaypointType.Exit, "Emergency Exit");
        
        Debug.Log("🏢 Created building waypoints template! Adjust positions as needed.");
    }
    
    /// <summary>
    /// Create waypoint at specific position
    /// </summary>
    public GameObject CreateWaypointAt(Vector3 position, WaypointType type, string customName = "")
    {
        // Create waypoint GameObject
        GameObject waypoint = GameObject.CreatePrimitive(waypointShape);
        waypoint.transform.position = position;
        waypoint.transform.localScale = waypointScale;
        
        // Set name
        string waypointName = customName;
        if (string.IsNullOrEmpty(waypointName))
        {
            waypointName = autoNumberWaypoints ? 
                $"{waypointBaseName}_{waypointCounter++}" : 
                waypointBaseName;
        }
        waypoint.name = waypointName;
        
        // Add Waypoint3D component
        Waypoint3D waypointComponent = waypoint.AddComponent<Waypoint3D>();
        waypointComponent.waypointName = waypointName;
        
        // Set waypoint properties based on type
        switch (type)
        {
            case WaypointType.Entrance:
                waypointComponent.isEntrance = true;
                waypointComponent.waypointColor = Color.green;
                break;
            case WaypointType.Exit:
                waypointComponent.isExit = true;
                waypointComponent.waypointColor = Color.red;
                break;
            default:
                waypointComponent.waypointColor = Color.blue;
                break;
        }
        
        // Apply material and color
        SetupWaypointVisuals(waypoint, waypointComponent.waypointColor);
        
        Debug.Log($"📍 Created {type} waypoint: {waypointName} at {position}");
        
        return waypoint;
    }
    
    /// <summary>
    /// Setup waypoint visual appearance
    /// </summary>
    void SetupWaypointVisuals(GameObject waypoint, Color color)
    {
        MeshRenderer renderer = waypoint.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            if (waypointMaterial != null)
            {
                // Use provided material
                Material instanceMaterial = new Material(waypointMaterial);
                instanceMaterial.color = color;
                renderer.material = instanceMaterial;
            }
            else
            {
                // Create new material with color (URP compatible)
                Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                newMaterial.color = color;
                // Set URP-specific properties via shader properties
                newMaterial.SetFloat("_Metallic", 0f);
                newMaterial.SetFloat("_Smoothness", 0.5f);
                renderer.material = newMaterial;
            }
        }
        
        // Add collider for better interaction (optional)
        Collider col = waypoint.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true; // Make it a trigger so it doesn't block movement
        }
    }
    
    /// <summary>
    /// Reset waypoint counter
    /// </summary>
    [ContextMenu("Reset Waypoint Counter")]
    public void ResetWaypointCounter()
    {
        waypointCounter = 1;
        Debug.Log("🔄 Reset waypoint counter to 1");
    }
    
    /// <summary>
    /// Find and setup WaypointManager3D
    /// </summary>
    [ContextMenu("Setup Waypoint Manager")]
    public void SetupWaypointManager()
    {
        WaypointManager3D manager = FindFirstObjectByType<WaypointManager3D>();
        if (manager == null)
        {
            GameObject managerObj = new GameObject("WaypointManager3D");
            manager = managerObj.AddComponent<WaypointManager3D>();
            Debug.Log("🎯 Created WaypointManager3D");
        }
        
        manager.RefreshWaypointList();
        manager.AutoDetectBuildingBounds();
        
        Debug.Log("✅ Waypoint Manager setup complete!");
    }
    
    void OnDrawGizmos()
    {
        // Draw waypoint creation preview
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 1f, "Waypoint Creator\n(Use context menu)");
        #endif
    }
}