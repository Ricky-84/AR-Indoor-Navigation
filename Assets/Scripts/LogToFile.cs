using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// Saves all debug logs to a file on the device for later retrieval
/// </summary>
public class LogToFile : MonoBehaviour
{
    private string logFilePath;
    private StringBuilder logBuilder = new StringBuilder();
    private int logCount = 0;

    void Awake()
    {
        // Create log file in persistent data path (accessible via USB)
        logFilePath = Path.Combine(Application.persistentDataPath, "debug_log.txt");

        // Clear old log
        File.WriteAllText(logFilePath, $"=== AR Navigation Debug Log ===\n");
        File.AppendAllText(logFilePath, $"Started: {System.DateTime.Now}\n");
        File.AppendAllText(logFilePath, $"Device: {SystemInfo.deviceModel}\n");
        File.AppendAllText(logFilePath, $"OS: {SystemInfo.operatingSystem}\n");
        File.AppendAllText(logFilePath, $"Graphics API: {SystemInfo.graphicsDeviceType}\n");
        File.AppendAllText(logFilePath, $"Log file location: {logFilePath}\n\n");

        // Subscribe to log messages
        Application.logMessageReceived += HandleLog;

        Debug.Log($"📝 LogToFile initialized - saving to: {logFilePath}");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logCount++;

        string timestamp = System.DateTime.Now.ToString("HH:mm:ss.fff");
        string typeStr = type.ToString().ToUpper();

        logBuilder.Clear();
        logBuilder.AppendLine($"[{timestamp}] [{typeStr}] {logString}");

        // For errors and exceptions, include stack trace
        if (type == LogType.Error || type == LogType.Exception)
        {
            logBuilder.AppendLine($"Stack trace:\n{stackTrace}");
        }

        logBuilder.AppendLine(); // Empty line between logs

        // Write to file immediately
        try
        {
            File.AppendAllText(logFilePath, logBuilder.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to write log: {e.Message}");
        }

        // Every 50 logs, remind user where the file is
        if (logCount % 50 == 0)
        {
            Debug.Log($"📝 {logCount} logs saved to: {logFilePath}");
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;

        // Write final summary
        File.AppendAllText(logFilePath, $"\n=== Log ended: {System.DateTime.Now} ===\n");
        File.AppendAllText(logFilePath, $"Total logs: {logCount}\n");

        Debug.Log($"📝 Final log saved with {logCount} entries");
    }

    void OnApplicationQuit()
    {
        OnDestroy();
    }
}
