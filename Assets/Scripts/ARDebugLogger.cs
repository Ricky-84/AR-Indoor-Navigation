using UnityEngine;

/// <summary>
/// Logs all errors with full stack traces to help diagnose issues
/// </summary>
public class ARDebugLogger : MonoBehaviour
{
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            Debug.Log($"=== ERROR CAUGHT ===");
            Debug.Log($"Message: {logString}");
            Debug.Log($"Stack Trace: {stackTrace}");
            Debug.Log($"===================");
        }
    }

    void Start()
    {
        Debug.Log("🔍 ARDebugLogger started - will log all errors with full details");
    }
}
