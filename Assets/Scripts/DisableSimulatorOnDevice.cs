using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

/// <summary>
/// Disables XR Device Simulator when running on actual device
/// The simulator is only for Unity Editor testing
/// </summary>
public class DisableSimulatorOnDevice : MonoBehaviour
{
    void Awake()
    {
        // Only run on actual device (not in editor)
        #if !UNITY_EDITOR
        DisableSimulator();
        #endif
    }

    void DisableSimulator()
    {
        // Find and disable XR Device Simulator
        XRDeviceSimulator simulator = FindFirstObjectByType<XRDeviceSimulator>();
        if (simulator != null)
        {
            Debug.Log("🔧 Disabling XR Device Simulator (running on real device)");
            simulator.gameObject.SetActive(false);
            Destroy(simulator.gameObject);
        }

        // Also find by name
        GameObject simObj = GameObject.Find("XR Device Simulator");
        if (simObj != null)
        {
            Debug.Log("🔧 Found and disabled XR Device Simulator GameObject");
            simObj.SetActive(false);
            Destroy(simObj);
        }
    }
}
