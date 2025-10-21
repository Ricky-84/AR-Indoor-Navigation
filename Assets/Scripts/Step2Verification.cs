using UnityEngine;
using UnityEngine.UI;

public class Step2Verification : MonoBehaviour
{
    [Header("Verification Results")]
    [SerializeField] private bool allComponentsFound = false;
    
    void Start()
    {
        StartCoroutine(VerifyUISetup());
    }
    
    System.Collections.IEnumerator VerifyUISetup()
    {
        yield return new WaitForSeconds(1f); // Wait for UI to be created
        
        Debug.Log("🔍 Starting Step 2 Verification...");
        
        bool canvasFound = VerifyCanvas();
        bool floorPlanFound = VerifyFloorPlan();
        bool userDotFound = VerifyUserDot();
        bool textFound = VerifyInstructionText();
        bool buttonFound = VerifyConfirmButton();
        bool managerFound = VerifyTapManager();
        bool coordinatorFound = VerifyCoordinator();
        
        allComponentsFound = canvasFound && floorPlanFound && userDotFound && 
                           textFound && buttonFound && managerFound && coordinatorFound;
        
        if (allComponentsFound)
        {
            Debug.Log("✅ STEP 2 VERIFICATION PASSED!");
            Debug.Log("🎉 All UI components created successfully!");
            Debug.Log("📱 You can now test by clicking on the light blue floor plan area.");
        }
        else
        {
            Debug.LogError("❌ STEP 2 VERIFICATION FAILED!");
            Debug.LogError("Some UI components are missing. Check the logs above.");
        }
    }
    
    bool VerifyCanvas()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Debug.Log("✅ Canvas found: " + canvas.name);
            return true;
        }
        Debug.LogError("❌ Canvas not found!");
        return false;
    }
    
    bool VerifyFloorPlan()
    {
        RawImage floorPlan = FindFirstObjectByType<RawImage>();
        if (floorPlan != null && floorPlan.name.Contains("FloorPlan"))
        {
            Debug.Log("✅ Floor Plan Image found: " + floorPlan.name);
            Debug.Log($"   - Size: {floorPlan.rectTransform.rect.size}");
            Debug.Log($"   - Has Texture: {floorPlan.texture != null}");
            return true;
        }
        Debug.LogError("❌ Floor Plan Image not found!");
        return false;
    }
    
    bool VerifyUserDot()
    {
        // Look for GameObject with UserLocation in name
        GameObject dotObject = GameObject.Find("UserLocationDot");
        if (dotObject != null && dotObject.GetComponent<Image>() != null)
        {
            Debug.Log("✅ User Location Dot found: " + dotObject.name);
            Debug.Log($"   - Color: {dotObject.GetComponent<Image>().color}");
            Debug.Log($"   - Initially Hidden: {!dotObject.activeSelf}");
            return true;
        }
        
        // Fallback: search through all Image components
        Image[] images = FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Image img in images)
        {
            if (img.name.Contains("UserLocation") || img.name.Contains("Dot"))
            {
                Debug.Log("✅ User Location Dot found (fallback): " + img.name);
                Debug.Log($"   - Color: {img.color}");
                Debug.Log($"   - Initially Hidden: {!img.gameObject.activeSelf}");
                return true;
            }
        }
        
        Debug.LogError("❌ User Location Dot not found!");
        return false;
    }
    
    bool VerifyInstructionText()
    {
        Text[] texts = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Text text in texts)
        {
            if (text.name.Contains("Instruction") || text.text.Contains("Tap your current"))
            {
                Debug.Log("✅ Instruction Text found: " + text.name);
                Debug.Log($"   - Text: '{text.text}'");
                return true;
            }
        }
        Debug.LogError("❌ Instruction Text not found!");
        return false;
    }
    
    bool VerifyConfirmButton()
    {
        // Look for GameObject with Button in name
        GameObject buttonObject = GameObject.Find("ConfirmButton");
        if (buttonObject != null && buttonObject.GetComponent<Button>() != null)
        {
            Button button = buttonObject.GetComponent<Button>();
            Text buttonText = button.GetComponentInChildren<Text>();
            Debug.Log("✅ Confirm Button found: " + button.name);
            if (buttonText != null)
                Debug.Log($"   - Button Text: '{buttonText.text}'");
            Debug.Log($"   - Initially Hidden: {!button.gameObject.activeSelf}");
            return true;
        }
        
        // Fallback: search through all Button components  
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button btn in buttons)
        {
            if (btn.name.Contains("Confirm") || btn.name.Contains("Button"))
            {
                Text buttonText = btn.GetComponentInChildren<Text>();
                Debug.Log("✅ Confirm Button found (fallback): " + btn.name);
                if (buttonText != null)
                    Debug.Log($"   - Button Text: '{buttonText.text}'");
                Debug.Log($"   - Initially Hidden: {!btn.gameObject.activeSelf}");
                return true;
            }
        }
        
        Debug.LogError("❌ Confirm Button not found!");
        return false;
    }
    
    bool VerifyTapManager()
    {
        TapToLocateManager manager = FindFirstObjectByType<TapToLocateManager>();
        if (manager != null)
        {
            Debug.Log("✅ TapToLocateManager found: " + manager.name);
            return true;
        }
        Debug.LogError("❌ TapToLocateManager not found!");
        return false;
    }
    
    bool VerifyCoordinator()
    {
        CoordinateConverter coordinator = FindFirstObjectByType<CoordinateConverter>();
        if (coordinator != null)
        {
            Debug.Log("✅ CoordinateConverter found on: " + coordinator.name);
            
            // Check if FloorPlanBounds is properly initialized
            var boundsField = coordinator.GetType().GetField("floorPlanBounds", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (boundsField != null)
            {
                var bounds = boundsField.GetValue(coordinator);
                if (bounds != null)
                {
                    Debug.Log("✅ FloorPlanBounds is initialized");
                }
                else
                {
                    Debug.LogWarning("⚠️ FloorPlanBounds is null - this may cause click errors");
                }
            }
            
            return true;
        }
        Debug.LogError("❌ CoordinateConverter not found!");
        return false;
    }
}