using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// Attach to AR Session GameObject to debug AR camera issues
/// Shows detailed logs about AR initialization
/// </summary>
public class ARCameraDebugger : MonoBehaviour
{
    private ARSession arSession;
    private ARCameraManager arCameraManager;
    private ARCameraBackground arCameraBackground;

    void Start()
    {
        Debug.Log("═══════════════════════════════════════");
        Debug.Log("🔍 AR CAMERA DEBUGGER STARTING");
        Debug.Log("═══════════════════════════════════════");

        // Find AR components
        arSession = FindFirstObjectByType<ARSession>();
        arCameraManager = FindFirstObjectByType<ARCameraManager>();
        arCameraBackground = FindFirstObjectByType<ARCameraBackground>();

        LogARStatus();

        // Subscribe to AR session state changes
        if (arSession != null)
        {
            ARSession.stateChanged += OnARSessionStateChanged;
        }

        // Check every second
        InvokeRepeating("LogARStatus", 1f, 2f);
    }

    void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        Debug.Log($"🔄 AR Session State Changed: {args.state}");

        switch (args.state)
        {
            case ARSessionState.None:
                Debug.LogWarning("AR Session: None (Not initialized)");
                break;
            case ARSessionState.Unsupported:
                Debug.LogError("❌ AR Session: UNSUPPORTED! This device doesn't support ARCore!");
                Debug.LogError("   Your phone may not be ARCore compatible.");
                break;
            case ARSessionState.CheckingAvailability:
                Debug.Log("⏳ AR Session: Checking ARCore availability...");
                break;
            case ARSessionState.NeedsInstall:
                Debug.LogWarning("⚠️ AR Session: ARCore needs to be installed!");
                break;
            case ARSessionState.Installing:
                Debug.Log("📥 AR Session: Installing ARCore...");
                break;
            case ARSessionState.Ready:
                Debug.Log("✅ AR Session: READY!");
                break;
            case ARSessionState.SessionInitializing:
                Debug.Log("🔧 AR Session: Initializing...");
                break;
            case ARSessionState.SessionTracking:
                Debug.Log("✅ AR Session: TRACKING! AR is working!");
                break;
        }
    }

    void LogARStatus()
    {
        Debug.Log("─────────────────────────────────────");
        Debug.Log("📊 AR STATUS CHECK:");

        // Check ARSession
        if (arSession == null)
        {
            Debug.LogError("❌ ARSession component NOT FOUND!");
        }
        else
        {
            Debug.Log($"✅ ARSession found: {arSession.gameObject.name}");
            Debug.Log($"   Enabled: {arSession.enabled}");
            Debug.Log($"   Attempting Update: {arSession.attemptUpdate}");
        }

        // Check ARCameraManager
        if (arCameraManager == null)
        {
            Debug.LogError("❌ ARCameraManager component NOT FOUND!");
        }
        else
        {
            Debug.Log($"✅ ARCameraManager found: {arCameraManager.gameObject.name}");
            Debug.Log($"   Enabled: {arCameraManager.enabled}");
            Debug.Log($"   Auto Focus: {arCameraManager.autoFocusRequested}");

            // Camera descriptor check removed (not needed for debugging)
        }

        // Check ARCameraBackground
        if (arCameraBackground == null)
        {
            Debug.LogError("❌ ARCameraBackground component NOT FOUND!");
            Debug.LogError("   This is why the screen is black!");
            Debug.LogError("   The camera feed isn't being rendered.");
        }
        else
        {
            Debug.Log($"✅ ARCameraBackground found: {arCameraBackground.gameObject.name}");
            Debug.Log($"   Enabled: {arCameraBackground.enabled}");
            Debug.Log($"   Using Custom Material: {arCameraBackground.useCustomMaterial}");
        }

        // Check Camera
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("❌ Main Camera NOT FOUND!");
        }
        else
        {
            Debug.Log($"✅ Main Camera found: {cam.gameObject.name}");
            Debug.Log($"   Clear Flags: {cam.clearFlags}");
            Debug.Log($"   Background Color: {cam.backgroundColor}");
        }

        Debug.Log("─────────────────────────────────────");
    }

    void OnDestroy()
    {
        if (arSession != null)
        {
            ARSession.stateChanged -= OnARSessionStateChanged;
        }
        CancelInvoke();
    }
}
