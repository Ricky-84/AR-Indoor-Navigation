using UnityEngine;

[System.Serializable]
public class WaypointData
{
    public string waypointName;
    public Vector3 worldPosition;
    public string description;
    public bool isEntrance;
    public bool isExit;
}

/// <summary>
/// 3D waypoint component - Place this on GameObjects in your 3D building scene
/// </summary>
public class Waypoint3D : MonoBehaviour
{
    [Header("Waypoint Info")]
    public string waypointName = "New Waypoint";
    [TextArea(2, 4)]
    public string description = "";
    public bool isEntrance = false;
    public bool isExit = false;
    
    [Header("Visual Settings")]
    public Color waypointColor = Color.blue;
    public float gizmoSize = 0.5f;
    
    [Header("Development Only")]
    public bool hideInBuild = true;
    
    void Start()
    {
        // Hide waypoint models in builds (keep only for development)
        if (hideInBuild && !Application.isEditor)
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null) renderer.enabled = false;
            
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }
    
    void OnDrawGizmos()
    {
        // Draw waypoint gizmo in scene view
        Gizmos.color = waypointColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * gizmoSize * 2);
        
        // Draw waypoint name
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * (gizmoSize * 2.5f), waypointName);
        #endif
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, gizmoSize * 1.2f);
    }
    
    /// <summary>
    /// Get waypoint data for 2D mapping
    /// </summary>
    public WaypointData GetWaypointData()
    {
        return new WaypointData
        {
            waypointName = waypointName,
            worldPosition = transform.position,
            description = description,
            isEntrance = isEntrance,
            isExit = isExit
        };
    }
    
    /// <summary>
    /// Set waypoint color based on type
    /// </summary>
    [ContextMenu("Auto Set Color")]
    void AutoSetColor()
    {
        if (isEntrance) waypointColor = Color.green;
        else if (isExit) waypointColor = Color.red;
        else waypointColor = Color.blue;
        
        // Apply color to mesh if available
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null && renderer.material != null)
        {
            renderer.material.color = waypointColor;
        }
    }
}