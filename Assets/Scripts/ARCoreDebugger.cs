using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;

/// <summary>
/// Diagnoses ARCore initialization issues and provides detailed logging
/// </summary>
public class ARCoreDebugger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔍 ARCoreDebugger starting diagnostic...");
        StartCoroutine(DiagnoseAR());
    }

    IEnumerator DiagnoseAR()
    {
        yield return new WaitForSeconds(0.5f);

        Debug.Log("=== AR DIAGNOSTIC START ===");

        // Check AR Session
        ARSession session = FindFirstObjectByType<ARSession>();
        if (session == null)
        {
            Debug.LogError("❌ AR Session not found in scene!");
        }
        else
        {
            Debug.Log($"✅ AR Session found: {session.gameObject.name}");
            Debug.Log($"   - Session State: {ARSession.state}");
            Debug.Log($"   - Session Active: {session.enabled}");
        }

        // Check AR Camera Manager
        ARCameraManager camManager = FindFirstObjectByType<ARCameraManager>();
        if (camManager == null)
        {
            Debug.LogError("❌ AR Camera Manager not found!");
        }
        else
        {
            Debug.Log($"✅ AR Camera Manager found");
            Debug.Log($"   - Enabled: {camManager.enabled}");
            Debug.Log($"   - Auto Focus: {camManager.autoFocusRequested}");
        }

        // Check AR Camera Background
        ARCameraBackground camBackground = FindFirstObjectByType<ARCameraBackground>();
        if (camBackground == null)
        {
            Debug.LogError("❌ AR Camera Background not found!");
        }
        else
        {
            Debug.Log($"✅ AR Camera Background found");
            Debug.Log($"   - Enabled: {camBackground.enabled}");
            Debug.Log($"   - Use Custom Material: {camBackground.useCustomMaterial}");
        }

        // Check XR Origin
        GameObject xrOrigin = GameObject.Find("XR Origin (Mobile AR)");
        if (xrOrigin == null)
        {
            xrOrigin = GameObject.Find("XR Origin");
        }
        if (xrOrigin == null)
        {
            Debug.LogError("❌ XR Origin not found!");
        }
        else
        {
            Debug.Log($"✅ XR Origin found: {xrOrigin.name}");
        }

        // Check Main Camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("❌ Main Camera not found!");
        }
        else
        {
            Debug.Log($"✅ Main Camera found");
            Debug.Log($"   - Clear Flags: {mainCam.clearFlags}");
            Debug.Log($"   - Background Color: {mainCam.backgroundColor}");
            Debug.Log($"   - Culling Mask: {mainCam.cullingMask}");
        }

        // Check if AR subsystems are available
        yield return new WaitForSeconds(2f);

        Debug.Log($"📱 Device Info:");
        Debug.Log($"   - Model: {SystemInfo.deviceModel}");
        Debug.Log($"   - OS: {SystemInfo.operatingSystem}");
        Debug.Log($"   - Graphics API: {SystemInfo.graphicsDeviceType}");
        Debug.Log($"   - Graphics Device: {SystemInfo.graphicsDeviceName}");

        // Monitor AR Session state changes
        StartCoroutine(MonitorARSession());

        Debug.Log("=== AR DIAGNOSTIC END ===");
    }

    IEnumerator MonitorARSession()
    {
        ARSessionState lastState = ARSession.state;
        Debug.Log($"🔄 Monitoring AR Session state (starting: {lastState})");

        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.5f);

            if (ARSession.state != lastState)
            {
                Debug.Log($"🔄 AR Session state changed: {lastState} → {ARSession.state}");
                lastState = ARSession.state;

                if (ARSession.state == ARSessionState.SessionInitializing)
                {
                    Debug.Log("⏳ ARCore is initializing...");
                }
                else if (ARSession.state == ARSessionState.Ready)
                {
                    Debug.Log("✅ ARCore is ready!");
                }
                else if (ARSession.state == ARSessionState.SessionTracking)
                {
                    Debug.Log("✅ ARCore is tracking!");
                    break;
                }
                else if (ARSession.state == ARSessionState.Unsupported)
                {
                    Debug.LogError("❌ ARCore is not supported on this device!");
                }
                else if (ARSession.state == ARSessionState.NeedsInstall)
                {
                    Debug.LogWarning("⚠️ ARCore needs to be installed!");
                }
            }
        }

        Debug.Log($"🏁 Final AR Session state: {ARSession.state}");
    }
}
