using UnityEngine;
using UnityEngine.UI;

public class CompilationTestUnity6 : MonoBehaviour
{
    [Header("Unity 6 API Test")]
    [SerializeField] private bool testOnStart = true;
    
    void Start()
    {
        if (testOnStart)
        {
            TestUnity6APIs();
        }
    }
    
    [ContextMenu("Test Unity 6 APIs")]
    public void TestUnity6APIs()
    {
        Debug.Log("🧪 Testing Unity 6 API compatibility...");
        
        // Test new FindFirstObjectByType API
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas != null)
        {
            Debug.Log("✅ FindFirstObjectByType<Canvas>() works correctly");
        }
        
        RawImage rawImage = FindFirstObjectByType<RawImage>();
        if (rawImage != null)
        {
            Debug.Log("✅ FindFirstObjectByType<RawImage>() works correctly");
        }
        
        // Test FindObjectsByType API
        Image[] images = FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"✅ FindObjectsByType<Image>() found {images.Length} images");
        
        Text[] texts = FindObjectsByType<Text>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"✅ FindObjectsByType<Text>() found {texts.Length} texts");
        
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log($"✅ FindObjectsByType<Button>() found {buttons.Length} buttons");
        
        Debug.Log("✅ All Unity 6 API tests passed!");
    }
    
    [ContextMenu("Test Floor Plan Components")]
    public void TestFloorPlanComponents()
    {
        Debug.Log("🏢 Testing floor plan specific components...");
        
        TapToLocateManager manager = FindFirstObjectByType<TapToLocateManager>();
        if (manager != null)
        {
            Debug.Log("✅ TapToLocateManager found");
        }
        
        CoordinateConverter converter = FindFirstObjectByType<CoordinateConverter>();
        if (converter != null)
        {
            Debug.Log("✅ CoordinateConverter found");
        }
        
        DirectFloorPlanLoader loader = FindFirstObjectByType<DirectFloorPlanLoader>();
        if (loader != null)
        {
            Debug.Log("✅ DirectFloorPlanLoader found");
        }

        Debug.Log("✅ All floor plan components test completed!");
    }
}