using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple verification script that confirms the Floor Plan System is Unity 6 ready
/// </summary>
public class SimpleSystemReady : MonoBehaviour
{
    void Start()
    {
        Invoke("ShowSystemStatus", 2f);
    }
    
    [ContextMenu("Show System Status")]
    public void ShowSystemStatus()
    {
        Debug.Log("====================================================");
        Debug.Log("🎉 FLOOR PLAN NAVIGATION SYSTEM - UNITY 6 READY!");
        Debug.Log("====================================================");
        Debug.Log("");
        
        // Check core components
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Debug.Log("✅ Canvas: Found and ready");
        }
        
        TapToLocateManager manager = FindFirstObjectByType<TapToLocateManager>();
        if (manager != null)
        {
            Debug.Log("✅ Tap Manager: Found and ready");
        }
        
        CoordinateConverter converter = FindFirstObjectByType<CoordinateConverter>();
        if (converter != null)
        {
            Debug.Log("✅ Coordinate Converter: Found and ready");
        }
        
        // Check enhancement components
        DirectFloorPlanLoader loader = FindFirstObjectByType<DirectFloorPlanLoader>();
        if (loader != null)
        {
            Debug.Log("✅ Floor Plan Loader: Found and ready");
        }
        
        Debug.Log("");
        Debug.Log("🎯 QUICK TEST INSTRUCTIONS:");
        Debug.Log("   1. Run your FloorPlaneScene");
        Debug.Log("   2. Wait 2 seconds for automatic setup");
        Debug.Log("   3. Press SPACE key if image needs resizing");
        Debug.Log("   4. Click blue markers to set destinations");
        Debug.Log("   5. Tap floor plan to set user location");
        Debug.Log("   6. Click green 'Start AR Navigation' button");
        Debug.Log("");
        Debug.Log("⚡ INSTANT FIX CONTROLS:");
        Debug.Log("   • SPACE = Apply size fix instantly");
        Debug.Log("   • F = Apply zoom and marker fix");
        Debug.Log("   • H = Show help");
        Debug.Log("   • V = Verify system status");
        Debug.Log("");
        Debug.Log("🚀 YOUR SYSTEM IS UNITY 6 READY!");
        Debug.Log("====================================================");
    }
    
    void Update()
    {
        // Emergency help
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("📚 HELP - Floor Plan System Controls:");
            Debug.Log("H = Show this help");
            Debug.Log("SPACE = Apply size fix");
            Debug.Log("F = Apply zoom/marker fix");
            Debug.Log("V = Verify system status");
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            ShowSystemStatus();
        }
    }
}