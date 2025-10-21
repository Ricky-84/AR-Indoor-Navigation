using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages all 3D waypoints and handles coordinate conversion to 2D floor plan
/// </summary>
public class WaypointManager3D : MonoBehaviour
{
    [Header("Waypoint Management")]
    public List<Waypoint3D> waypoints = new List<Waypoint3D>();
    
    [Header("Building Bounds - Set to match your 3D building size")]
    public Bounds buildingBounds = new Bounds(Vector3.zero, new Vector3(120, 10, 80));
    public bool autoDetectBounds = true;
    
    [Header("Floor Plan Mapping - Match your 2D image size")]
    public Vector2 floorPlanSize = new Vector2(600, 400);
    
    [Header("Debug")]
    public bool showDebugInfo = true;
    
    void Start()
    {
        RefreshWaypointList();
        
        if (autoDetectBounds)
        {
            AutoDetectBuildingBounds();
        }
    }
    
    /// <summary>
    /// Find all waypoints in the scene
    /// </summary>
    [ContextMenu("Refresh Waypoint List")]
    public void RefreshWaypointList()
    {
        waypoints.Clear();
        waypoints.AddRange(FindObjectsByType<Waypoint3D>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        
        if (showDebugInfo)
        {
            Debug.Log($"📍 Found {waypoints.Count} waypoints in scene");
            foreach (var waypoint in waypoints)
            {
                Debug.Log($"   • {waypoint.waypointName} at {waypoint.transform.position}");
            }
        }
    }
    
    /// <summary>
    /// Auto-detect building bounds based on waypoint positions
    /// </summary>
    [ContextMenu("Auto Detect Building Bounds")]
    public void AutoDetectBuildingBounds()
    {
        RefreshWaypointList();
        
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("No waypoints found to calculate bounds!");
            return;
        }
        
        Vector3 min = waypoints[0].transform.position;
        Vector3 max = waypoints[0].transform.position;
        
        foreach (var waypoint in waypoints)
        {
            Vector3 pos = waypoint.transform.position;
            
            if (pos.x < min.x) min.x = pos.x;
            if (pos.y < min.y) min.y = pos.y;
            if (pos.z < min.z) min.z = pos.z;
            
            if (pos.x > max.x) max.x = pos.x;
            if (pos.y > max.y) max.y = pos.y;
            if (pos.z > max.z) max.z = pos.z;
        }
        
        // Add some padding
        Vector3 padding = new Vector3(10, 5, 10);
        min -= padding;
        max += padding;
        
        Vector3 center = (min + max) * 0.5f;
        Vector3 size = max - min;
        
        buildingBounds = new Bounds(center, size);
        
        if (showDebugInfo)
        {
            Debug.Log($"🏢 Auto-detected building bounds: Center={center}, Size={size}");
        }
    }
    
    /// <summary>
    /// Generate 2D coordinates for all waypoints
    /// </summary>
    [ContextMenu("Generate 2D Coordinates")]
    public void Generate2DCoordinates()
    {
        RefreshWaypointList();
        
        Debug.Log("=== 3D TO 2D WAYPOINT MAPPING ===");
        Debug.Log($"Building Bounds: {buildingBounds}");
        Debug.Log($"Floor Plan Size: {floorPlanSize}");
        Debug.Log("");
        
        foreach (var waypoint in waypoints)
        {
            Vector2 floorPlanPos = WorldTo2DFloorPlan(waypoint.transform.position);
            Vector2 normalizedPos = new Vector2(
                floorPlanPos.x / floorPlanSize.x,
                floorPlanPos.y / floorPlanSize.y
            );
            
            Debug.Log($"📍 Waypoint '{waypoint.waypointName}':");
            Debug.Log($"   3D Position: {waypoint.transform.position}");
            Debug.Log($"   2D Position: {floorPlanPos}");
            Debug.Log($"   Normalized: {normalizedPos}");
            Debug.Log("");
        }
        
        // Generate code for DirectFloorPlanLoader
        GenerateWaypointCode();
    }
    
    /// <summary>
    /// Convert 3D world position to 2D floor plan coordinates
    /// </summary>
    public Vector2 WorldTo2DFloorPlan(Vector3 worldPos)
    {
        // Normalize position within building bounds
        float normalizedX = (worldPos.x - buildingBounds.min.x) / buildingBounds.size.x;
        float normalizedZ = (worldPos.z - buildingBounds.min.z) / buildingBounds.size.z;
        
        // Clamp to valid range
        normalizedX = Mathf.Clamp01(normalizedX);
        normalizedZ = Mathf.Clamp01(normalizedZ);
        
        // Convert to floor plan pixel coordinates
        float x = normalizedX * floorPlanSize.x;
        float y = normalizedZ * floorPlanSize.y;
        
        return new Vector2(x, y);
    }
    
    /// <summary>
    /// Generate code for pasting into DirectFloorPlanLoader
    /// </summary>
    [ContextMenu("Generate Waypoint Code")]
    public void GenerateWaypointCode()
    {
        RefreshWaypointList();
        
        Debug.Log("=== COPY THIS CODE TO DirectFloorPlanLoader.cs ===");
        Debug.Log("Replace the destinations array with this code:");
        Debug.Log("");
        Debug.Log("destinations = new DestinationInfo[]");
        Debug.Log("{");
        
        foreach (var waypoint in waypoints)
        {
            Vector2 floorPlanPos = WorldTo2DFloorPlan(waypoint.transform.position);
            Vector2 normalizedPos = new Vector2(
                floorPlanPos.x / floorPlanSize.x,
                floorPlanPos.y / floorPlanSize.y
            );
            
            string colorType = waypoint.isEntrance ? "entrance" : waypoint.isExit ? "exit" : "normal";
            Debug.Log($"    new DestinationInfo {{ name = \"{waypoint.waypointName}\", normalizedPos = new Vector2({normalizedPos.x:F3}f, {normalizedPos.y:F3}f) }}, // {colorType}");
        }
        
        Debug.Log("};");
        Debug.Log("");
        Debug.Log("=== END CODE ===");
    }
    
    /// <summary>
    /// Get all waypoint data for external use
    /// </summary>
    public List<WaypointData> GetAllWaypointData()
    {
        RefreshWaypointList();
        List<WaypointData> dataList = new List<WaypointData>();
        
        foreach (var waypoint in waypoints)
        {
            dataList.Add(waypoint.GetWaypointData());
        }
        
        return dataList;
    }
    
    /// <summary>
    /// Create waypoint prefabs for easy placement
    /// </summary>
    [ContextMenu("Create Waypoint Prefab")]
    public void CreateWaypointPrefab()
    {
        // Create a simple waypoint GameObject
        GameObject waypointPrefab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        waypointPrefab.name = "Waypoint_Prefab";
        
        // Scale it down
        waypointPrefab.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        
        // Add Waypoint3D component
        Waypoint3D waypointComponent = waypointPrefab.AddComponent<Waypoint3D>();
        waypointComponent.waypointName = "New Waypoint";
        waypointComponent.waypointColor = Color.blue;
        
        // Set material color
        MeshRenderer renderer = waypointPrefab.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Color.blue;
            renderer.material = mat;
        }
        
        Debug.Log("🎯 Created waypoint prefab! Duplicate this and position around your building.");
    }
    
    void OnDrawGizmos()
    {
        // Draw building bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(buildingBounds.center, buildingBounds.size);
        
        // Draw coordinate system
        Gizmos.color = Color.red;
        Gizmos.DrawLine(buildingBounds.min, buildingBounds.min + Vector3.right * 5);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(buildingBounds.min, buildingBounds.min + Vector3.forward * 5);
        
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(buildingBounds.center + Vector3.up * (buildingBounds.size.y * 0.5f + 2), $"Building Bounds\n{buildingBounds.size.x:F1} x {buildingBounds.size.z:F1}m");
        #endif
    }
}