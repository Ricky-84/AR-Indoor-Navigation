using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSetupTest : MonoBehaviour
{
    private ARPlaneManager planeManager;
    
    void Start()
    {
        planeManager = FindFirstObjectByType<ARPlaneManager>();
        
        if (planeManager == null)
            Debug.LogError("AR Plane Manager not found!");
        else
            Debug.Log("AR Setup complete - looking for planes...");
    }
    
    void Update()
    {
        if (planeManager != null && planeManager.trackables.count > 0)
        {
            Debug.Log($"Found {planeManager.trackables.count} planes");
        }
    }
}
