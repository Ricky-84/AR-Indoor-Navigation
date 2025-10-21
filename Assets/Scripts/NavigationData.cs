using UnityEngine;

public static class NavigationData
{
    [Header("Position Data")]
    public static Vector3 userStartPosition = Vector3.zero;
    public static Vector3 destinationPosition = Vector3.zero;
    
    [Header("Destination Info")]
    public static string destinationName = "";
    
    [Header("State")]
    public static bool hasValidData = false;
    
    public static void Reset()
    {
        userStartPosition = Vector3.zero;
        destinationPosition = Vector3.zero;
        destinationName = "";
        hasValidData = false;
    }
    
    public static void LogData()
    {
        Debug.Log($"NavigationData Status:");
        Debug.Log($"  - Has Valid Data: {hasValidData}");
        Debug.Log($"  - User Start Position: {userStartPosition}");
        Debug.Log($"  - Destination Position: {destinationPosition}");
        Debug.Log($"  - Destination Name: {destinationName}");
    }
}