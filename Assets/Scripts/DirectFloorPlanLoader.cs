using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DirectFloorPlanLoader : MonoBehaviour
{
    [Header("Automatic Floor Plan Loading")]
    [SerializeField] private bool autoLoadOnStart = true;
    
    void Start()
    {
        if (autoLoadOnStart)
        {
            LoadFloorPlanAutomatically();
        }
    }
    
    [ContextMenu("Load Floor Plan Automatically")]
    public void LoadFloorPlanAutomatically()
    {
        Debug.Log("🔍 Searching for floor plan image in project...");
        
        Texture2D floorPlanTexture = FindFloorPlanInProject();
        
        if (floorPlanTexture != null)
        {
            ApplyFloorPlanTexture(floorPlanTexture);
            Debug.Log($"✅ Automatically loaded floor plan: {floorPlanTexture.name}");
            
            // Setup destinations after loading the image
            SetupDestinations();
        }
        else
        {
            Debug.LogWarning("⚠️ No floor plan image found. Creating placeholder.");
            CreatePlaceholderWithDestinations();
        }
    }
    
    Texture2D FindFloorPlanInProject()
    {
        // First try Resources folder
        Texture2D texture = Resources.Load<Texture2D>("AcademicBlockFloorPlan");
        if (texture != null)
        {
            Debug.Log("✅ Found floor plan in Resources folder");
            return texture;
        }
        
#if UNITY_EDITOR
        // Search entire project for floor plan images
        string[] searchTerms = { "AcademicBlock", "FloorPlan", "Academic", "Block", "floor", "plan" };
        
        foreach (string searchTerm in searchTerms)
        {
            string[] guids = AssetDatabase.FindAssets($"{searchTerm} t:Texture2D");
            
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Texture2D foundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                
                if (foundTexture != null)
                {
                    Debug.Log($"✅ Found floor plan at: {path}");
                    return foundTexture;
                }
            }
        }
        
        // If not found, look for any texture in Resources
        string[] allTextureGUIDs = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Resources" });
        if (allTextureGUIDs.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(allTextureGUIDs[0]);
            Texture2D foundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (foundTexture != null)
            {
                Debug.Log($"✅ Using first texture found in Resources: {foundTexture.name}");
                return foundTexture;
            }
        }
#endif
        
        return null;
    }
    
    void ApplyFloorPlanTexture(Texture2D texture)
    {
        RawImage floorPlanImage = FindFirstObjectByType<RawImage>();
        if (floorPlanImage != null)
        {
            floorPlanImage.texture = texture;
            Debug.Log("✅ Applied real floor plan texture to UI");
            
            // Update coordinate system for real building
            UpdateCoordinateSystem();
        }
        else
        {
            Debug.LogError("❌ No RawImage found! Please run QuickFloorPlanSetup first.");
        }
    }
    
    void UpdateCoordinateSystem()
    {
        CoordinateConverter coordinator = FindFirstObjectByType<CoordinateConverter>();
        if (coordinator != null)
        {
            // Create new bounds matching SampleScene WaypointManager3D settings
            // Building Bounds: Center=(-0.80, 0, -1.48), Extents=(150, 5, 70)
            // Actual size: 300m x 140m
            FloorPlanBounds newBounds = new FloorPlanBounds();
            newBounds.realWorldSize = new Vector2(300f, 140f);
            newBounds.worldOrigin = new Vector3(-150.80000305f, 0f, -71.4750004f);
            newBounds.floorHeight = 0f;

            // Update using reflection
            var boundsField = coordinator.GetType().GetField("floorPlanBounds",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (boundsField != null)
            {
                boundsField.SetValue(coordinator, newBounds);
                Debug.Log($"✅ Updated coordinate system for 300m x 140m academic building");
            }
        }
    }
    
    void SetupDestinations()
    {
        Debug.Log("🎯 Creating destination markers for your academic building...");
        
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) return;
        
        // Clear existing destination markers
        ClearExistingDestinations();
        
        // Create destination points based on your actual floor plan layout
        DestinationInfo[] destinations = GetAcademicBuildingDestinations();
        
        foreach (DestinationInfo dest in destinations)
        {
            CreateDestinationMarker(dest, canvas);
        }
        
        Debug.Log($"✅ Created {destinations.Length} destination markers");
    }
    
    void ClearExistingDestinations()
    {
        GameObject[] markers = MarkerHelper.FindAllMarkers();
        foreach (GameObject marker in markers)
        {
            DestroyImmediate(marker);
        }
    }
    
    DestinationInfo[] GetAcademicBuildingDestinations()
    {
        // Generated from 3D waypoints with adjusted building bounds (300m x 140m)
        return new DestinationInfo[]
        {
            new DestinationInfo { name = "A103", normalizedPos = new Vector2(0.261f, 0.782f) },
            new DestinationInfo { name = "Law-1", normalizedPos = new Vector2(0.447f, 0.218f) },
            new DestinationInfo { name = "A102", normalizedPos = new Vector2(0.176f, 0.782f) },
            new DestinationInfo { name = "C104", normalizedPos = new Vector2(0.345f, 0.261f) },
            new DestinationInfo { name = "Anugraha Hall-1", normalizedPos = new Vector2(0.639f, 0.263f) },
            new DestinationInfo { name = "Faculty-1B", normalizedPos = new Vector2(0.286f, 0.587f) },
            new DestinationInfo { name = "Seminar Hall-1", normalizedPos = new Vector2(0.724f, 0.263f) },
            new DestinationInfo { name = "Stairs-1", normalizedPos = new Vector2(0.148f, 0.476f) },
            new DestinationInfo { name = "Faculty-2A", normalizedPos = new Vector2(0.818f, 0.550f) },
            new DestinationInfo { name = "Faculty-1A", normalizedPos = new Vector2(0.191f, 0.495f) },
            new DestinationInfo { name = "Back Entrance", normalizedPos = new Vector2(0.906f, 0.368f) },
            new DestinationInfo { name = "Faculty-2B", normalizedPos = new Vector2(0.727f, 0.583f) },
            new DestinationInfo { name = "Stairs-4", normalizedPos = new Vector2(0.852f, 0.560f) },
            new DestinationInfo { name = "A104", normalizedPos = new Vector2(0.345f, 0.782f) },
            new DestinationInfo { name = "D103", normalizedPos = new Vector2(0.679f, 0.780f) },
            new DestinationInfo { name = "Boys Toilet-1", normalizedPos = new Vector2(0.154f, 0.303f) },
            new DestinationInfo { name = "Law-2", normalizedPos = new Vector2(0.506f, 0.236f) },
            new DestinationInfo { name = "Faculty-1D", normalizedPos = new Vector2(0.423f, 0.533f) },
            new DestinationInfo { name = "D102", normalizedPos = new Vector2(0.667f, 0.780f) },
            new DestinationInfo { name = "C103", normalizedPos = new Vector2(0.261f, 0.261f) },
            new DestinationInfo { name = "Main Entrance-2", normalizedPos = new Vector2(0.094f, 0.358f) },
            new DestinationInfo { name = "Stairs-3", normalizedPos = new Vector2(0.483f, 0.280f) },
            new DestinationInfo { name = "D104/5", normalizedPos = new Vector2(0.759f, 0.780f) },
            new DestinationInfo { name = "Faculty-1C", normalizedPos = new Vector2(0.286f, 0.448f) },
            new DestinationInfo { name = "Department Office", normalizedPos = new Vector2(0.595f, 0.510f) },
            new DestinationInfo { name = "C102", normalizedPos = new Vector2(0.176f, 0.261f) },
            new DestinationInfo { name = "D106", normalizedPos = new Vector2(0.836f, 0.780f) },
            new DestinationInfo { name = "Anugraha Hall-2", normalizedPos = new Vector2(0.710f, 0.263f) },
            new DestinationInfo { name = "D108", normalizedPos = new Vector2(0.847f, 0.704f) },
            new DestinationInfo { name = "D101", normalizedPos = new Vector2(0.602f, 0.782f) },
            new DestinationInfo { name = "Faculty-2C", normalizedPos = new Vector2(0.727f, 0.458f) },
            new DestinationInfo { name = "Law-3", normalizedPos = new Vector2(0.566f, 0.218f) },
            new DestinationInfo { name = "Cafeteria", normalizedPos = new Vector2(0.503f, 0.616f) },
            new DestinationInfo { name = "Boys Toilet-2", normalizedPos = new Vector2(0.865f, 0.737f) },
            new DestinationInfo { name = "Girls Toilet-2", normalizedPos = new Vector2(0.865f, 0.303f) },
            new DestinationInfo { name = "Main Entrance-1", normalizedPos = new Vector2(0.094f, 0.682f) },
            new DestinationInfo { name = "Stairs-2", normalizedPos = new Vector2(0.532f, 0.638f) },
            new DestinationInfo { name = "Girls Toilet-1", normalizedPos = new Vector2(0.154f, 0.731f) },
            new DestinationInfo { name = "Electric Room", normalizedPos = new Vector2(0.473f, 0.774f) },
            new DestinationInfo { name = "Seminar Hall-2", normalizedPos = new Vector2(0.795f, 0.263f) }
        };
    }
    
    void CreateDestinationMarker(DestinationInfo destination, Canvas canvas)
    {
        // Find the floor plan to parent markers to it
        RawImage floorPlan = FindFirstObjectByType<RawImage>();
        if (floorPlan == null)
        {
            Debug.LogError("❌ Cannot create markers - FloorPlanImage not found!");
            return;
        }

        GameObject markerObj = new GameObject($"Dest_{destination.name}");
        markerObj.transform.SetParent(floorPlan.transform, false); // Parent to floor plan, not canvas

        // Create bright, visible circular marker
        Image marker = markerObj.AddComponent<Image>();
        marker.color = new Color(1f, 0.2f, 0.2f, 0.95f); // Bright red for visibility

        // Create circle sprite
        Texture2D circleTexture = CreateCircleTexture(32);
        Sprite circleSprite = Sprite.Create(circleTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
        marker.sprite = circleSprite;

        RectTransform rect = marker.rectTransform;
        rect.sizeDelta = new Vector2(18, 18);

        // Flip both X and Y coordinates to match floor plan orientation
        Vector2 flipped = new Vector2(
            1f - destination.normalizedPos.x,
            1f - destination.normalizedPos.y
        );

        rect.anchorMin = flipped;
        rect.anchorMax = flipped;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero; // Zero offset from anchor

        Debug.Log($"📍 Created marker for {destination.name} at normalized position {destination.normalizedPos}");
        
        // Add click functionality
        Button button = markerObj.AddComponent<Button>();
        button.onClick.AddListener(() => SelectDestination(destination));
        
        // Add hover effect
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1f, 0.5f, 0.5f, 1f); // Lighter red on hover
        colors.pressedColor = new Color(0.8f, 0.1f, 0.1f, 1f); // Darker red when pressed
        button.colors = colors;
        
        // Add text label for better identification
        CreateMarkerLabel(markerObj, destination.name);
        
        Debug.Log($"📍 Created visible marker for {destination.name}");
    }
    
    Vector2 NormalizedToScreenPosition(Vector2 normalized)
    {
        RawImage floorPlan = FindFirstObjectByType<RawImage>();
        if (floorPlan != null)
        {
            RectTransform rect = floorPlan.rectTransform;
            Vector2 localPos = new Vector2(
                (normalized.x - 0.5f) * rect.rect.width,
                (normalized.y - 0.5f) * rect.rect.height
            );
            Vector3 worldPos = rect.TransformPoint(localPos);
            return RectTransformUtility.WorldToScreenPoint(null, worldPos);
        }
        return Vector2.zero;
    }
    
    void SelectDestination(DestinationInfo destination)
    {
        Debug.Log($"🎯 Selected destination: {destination.name}");
        
        // Convert to world position
        Vector2 screenPos = NormalizedToScreenPosition(destination.normalizedPos);
        CoordinateConverter coordinator = FindFirstObjectByType<CoordinateConverter>();
        if (coordinator != null)
        {
            Vector3 worldPos = coordinator.UIToWorldPosition(screenPos);
            
            // Update TapToLocateManager
            TapToLocateManager manager = FindFirstObjectByType<TapToLocateManager>();
            if (manager != null)
            {
                manager.SetDestination(worldPos, destination.name);
            }
        }
    }
    
    Texture2D CreateCircleTexture(int size)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 1;
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 pos = new Vector2(x, y);
                float distance = Vector2.Distance(pos, center);
                pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
            }
        }
        
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
    
    void CreatePlaceholderWithDestinations()
    {
        Debug.Log("📋 No real floor plan found, but setting up destinations on placeholder");
        UpdateCoordinateSystem();
        SetupDestinations();
    }
    
    void CreateMarkerLabel(GameObject markerObj, string labelText)
    {
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(markerObj.transform, false);

        Text label = labelObj.AddComponent<Text>();
        label.text = labelText.Replace(" ", "\n"); // Break long names into multiple lines
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 10;
        label.color = Color.black;
        label.alignment = TextAnchor.MiddleCenter;

        // Add outline for better readability
        Outline outline = labelObj.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(1, 1);

        RectTransform labelRect = label.rectTransform;
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.pivot = new Vector2(0.5f, 0f); // Pivot at bottom center
        labelRect.anchoredPosition = new Vector2(0, 12); // Position above the marker
        labelRect.sizeDelta = new Vector2(60, 20);

        // No background - using outline instead for better visibility
    }
    
    [ContextMenu("Recreate Destination Markers")]
    public void RecreateDestinationMarkers()
    {
        Debug.Log("🔄 Recreating destination markers...");
        ClearExistingDestinations();
        SetupDestinations();
    }
    
    [System.Serializable]
    public class DestinationInfo
    {
        public string name;
        public Vector2 normalizedPos;
    }
}