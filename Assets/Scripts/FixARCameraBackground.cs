using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections;

/// <summary>
/// Ensures AR Camera Background is properly configured
/// </summary>
public class FixARCameraBackground : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(FixBackground());
    }

    IEnumerator FixBackground()
    {
        yield return new WaitForSeconds(1f);

        ARCameraBackground camBackground = FindFirstObjectByType<ARCameraBackground>();
        if (camBackground == null)
        {
            Debug.LogError("❌ AR Camera Background not found!");
            yield break;
        }

        Debug.Log($"🔧 Checking AR Camera Background...");
        Debug.Log($"   - Enabled: {camBackground.enabled}");
        Debug.Log($"   - Use Custom Material: {camBackground.useCustomMaterial}");
        Debug.Log($"   - Material: {camBackground.material}");
        Debug.Log($"   - Custom Material: {camBackground.customMaterial}");

        // Make sure it's not using custom material
        if (camBackground.useCustomMaterial)
        {
            Debug.Log("🔧 Disabling custom material...");
            camBackground.useCustomMaterial = false;
        }

        // Force re-enable
        camBackground.enabled = false;
        yield return null;
        camBackground.enabled = true;

        Debug.Log("✅ AR Camera Background refreshed");

        // Check Camera clear flags
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Debug.Log($"📷 Camera Clear Flags: {mainCam.clearFlags}");
            if (mainCam.clearFlags != CameraClearFlags.Color && mainCam.clearFlags != CameraClearFlags.SolidColor)
            {
                Debug.Log("🔧 Setting camera clear flags to SolidColor");
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = Color.black;
            }
        }
    }
}
