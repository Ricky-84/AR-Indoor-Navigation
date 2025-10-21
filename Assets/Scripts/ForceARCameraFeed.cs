using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering;
using System.Collections;

/// <summary>
/// Forces AR camera feed to render properly by ensuring correct configuration
/// </summary>
public class ForceARCameraFeed : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(ConfigureARCamera());
    }

    IEnumerator ConfigureARCamera()
    {
        // Wait for AR to fully initialize
        yield return new WaitForSeconds(2f);

        Debug.Log("🔧 Force AR Camera Feed - Starting configuration...");

        // Find components
        ARCameraManager camManager = FindFirstObjectByType<ARCameraManager>();
        ARCameraBackground camBackground = FindFirstObjectByType<ARCameraBackground>();
        Camera mainCamera = Camera.main;

        if (camManager == null)
        {
            Debug.LogError("❌ ARCameraManager not found!");
            yield break;
        }

        if (camBackground == null)
        {
            Debug.LogError("❌ ARCameraBackground not found!");
            yield break;
        }

        if (mainCamera == null)
        {
            Debug.LogError("❌ Main Camera not found!");
            yield break;
        }

        Debug.Log("✅ All AR components found");

        // Configure Camera
        Debug.Log("🔧 Configuring camera settings...");
        mainCamera.clearFlags = CameraClearFlags.SolidColor;
        mainCamera.backgroundColor = Color.black;
        mainCamera.depth = 0;

        // Configure AR Camera Background
        Debug.Log("🔧 Configuring AR Camera Background...");
        camBackground.useCustomMaterial = false;

        // Force disable and re-enable to refresh
        camBackground.enabled = false;
        camManager.enabled = false;
        yield return null;

        camManager.enabled = true;
        camBackground.enabled = true;

        Debug.Log("✅ AR Camera Background re-enabled");

        // Wait and check if texture is updating
        yield return new WaitForSeconds(1f);

        // Subscribe to frame received event
        camManager.frameReceived += OnCameraFrameReceived;

        Debug.Log("🔧 Subscribed to camera frame events");
        Debug.Log($"📷 Camera state: ClearFlags={mainCamera.clearFlags}, Depth={mainCamera.depth}");
        Debug.Log($"📷 AR Background: Enabled={camBackground.enabled}, UseCustomMaterial={camBackground.useCustomMaterial}");
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log($"📸 Camera frame received! Timestamp: {args.timestampNs}");

        // Unsubscribe after first frame
        ARCameraManager camManager = FindFirstObjectByType<ARCameraManager>();
        if (camManager != null)
        {
            camManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    void OnDestroy()
    {
        ARCameraManager camManager = FindFirstObjectByType<ARCameraManager>();
        if (camManager != null)
        {
            camManager.frameReceived -= OnCameraFrameReceived;
        }
    }
}
