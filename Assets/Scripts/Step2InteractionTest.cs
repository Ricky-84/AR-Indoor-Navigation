using UnityEngine;

public class Step2InteractionTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool enableTestLogging = true;
    
    void Start()
    {
        if (enableTestLogging)
        {
            StartCoroutine(WaitAndTestInteraction());
        }
    }
    
    System.Collections.IEnumerator WaitAndTestInteraction()
    {
        yield return new WaitForSeconds(2f);
        
        Debug.Log("🧪 STEP 2 INTERACTION TEST");
        Debug.Log("📱 The floor plan should be clickable now!");
        Debug.Log("🎯 Try clicking anywhere on the light blue floor plan area.");
        Debug.Log("✅ Expected result: Red dot appears at click location");
        Debug.Log("✅ Expected result: Text changes to show location is set");
        Debug.Log("✅ Expected result: Green 'Start AR Navigation' button appears");
    }
    
    void Update()
    {
        if (enableTestLogging && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;
            Debug.Log($"🖱️ Mouse clicked at screen position: {mousePos}");
            
            // Check if we can find the coordinate converter
            CoordinateConverter coordinator = FindFirstObjectByType<CoordinateConverter>();
            if (coordinator != null)
            {
                Debug.Log("✅ CoordinateConverter found - click should be processed");
                
                // Test if the position is valid
                if (coordinator.IsPositionValid(mousePos))
                {
                    Vector3 worldPos = coordinator.UIToWorldPosition(mousePos);
                    Debug.Log($"✅ Click converted to world position: {worldPos}");
                }
                else
                {
                    Debug.Log("ℹ️ Click was outside floor plan area");
                }
            }
            else
            {
                Debug.LogError("❌ CoordinateConverter not found!");
            }
        }
    }
}