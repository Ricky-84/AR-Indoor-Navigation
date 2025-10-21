using UnityEngine;

/// <summary>
/// Helper class to find marker GameObjects without using tags
/// </summary>
public static class MarkerHelper
{
    /// <summary>
    /// Find all marker GameObjects by name pattern instead of tag
    /// Replaces: GameObject.FindGameObjectsWithTag("DestinationMarker")
    /// </summary>
    public static GameObject[] FindAllMarkers()
    {
        // Find all GameObjects in scene using Unity 6 API
        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Filter for objects that look like markers
        System.Collections.Generic.List<GameObject> markers = new System.Collections.Generic.List<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Check if name starts with "Marker_" or "Dest_"
            if (obj.name.StartsWith("Marker_") || obj.name.StartsWith("Dest_"))
            {
                markers.Add(obj);
            }
        }

        return markers.ToArray();
    }
}
