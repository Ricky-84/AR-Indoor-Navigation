using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Shows debug logs on screen for mobile testing
/// </summary>
public class OnScreenDebugLog : MonoBehaviour
{
    private Text debugText;
    private Queue<string> logQueue = new Queue<string>();
    private int maxLines = 20;

    void Awake()
    {
        // Create canvas for debug text
        GameObject canvasObj = new GameObject("DebugCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(canvasObj);

        // Create text object
        GameObject textObj = new GameObject("DebugText");
        textObj.transform.SetParent(canvasObj.transform, false);

        debugText = textObj.AddComponent<Text>();
        debugText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        debugText.fontSize = 14;
        debugText.color = Color.white;
        debugText.alignment = TextAnchor.UpperLeft;

        // Add shadow for readability
        Shadow shadow = textObj.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(1, -1);

        RectTransform rect = debugText.rectTransform;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(1, 1);
        rect.offsetMin = new Vector2(10, 10);
        rect.offsetMax = new Vector2(-10, -10);

        // Subscribe to log messages
        Application.logMessageReceived += HandleLog;

        Debug.Log("🟢 OnScreenDebugLog initialized");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string prefix = "";
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                prefix = "<color=red>[ERROR]</color> ";
                break;
            case LogType.Warning:
                prefix = "<color=yellow>[WARN]</color> ";
                break;
            case LogType.Log:
                prefix = "<color=white>[LOG]</color> ";
                break;
        }

        string logEntry = prefix + logString;

        // For errors, include first line of stack trace
        if (type == LogType.Error || type == LogType.Exception)
        {
            string[] stackLines = stackTrace.Split('\n');
            if (stackLines.Length > 0)
            {
                logEntry += "\n  " + stackLines[0];
            }
        }

        logQueue.Enqueue(logEntry);

        // Keep only last N lines
        while (logQueue.Count > maxLines)
        {
            logQueue.Dequeue();
        }

        // Update display
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (debugText != null)
        {
            debugText.text = string.Join("\n", logQueue);
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
