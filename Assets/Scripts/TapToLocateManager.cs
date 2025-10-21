using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class TapToLocateManager : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private RawImage floorPlanImage;
    [SerializeField] private GameObject userLocationDot;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Text instructionText;
    
    [Header("Coordinate System")]
    [SerializeField] private CoordinateConverter coordinateConverter;
    
    [Header("Room Reference Points")]
    [SerializeField] private Transform[] roomMarkers;
    [SerializeField] private string[] roomNames;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = true;
    
    private Vector3 selectedWorldPosition;
    private bool locationSelected = false;
    private bool destinationSelected = false;
    private Vector3 destinationWorldPosition;
    private string selectedDestinationName;
    
    void Start()
    {
        AutoAssignReferences();
        InitializeUI();
        SetupEventHandlers();
    }

    void AutoAssignReferences()
    {
        // Auto-find missing references
        if (floorPlanImage == null)
            floorPlanImage = FindFirstObjectByType<RawImage>();

        if (coordinateConverter == null)
        {
            coordinateConverter = FindFirstObjectByType<CoordinateConverter>();
            if (coordinateConverter == null)
            {
                Debug.LogError("❌ CoordinateConverter not found! Creating one...");
                GameObject converterObj = new GameObject("CoordinateConverter");
                coordinateConverter = converterObj.AddComponent<CoordinateConverter>();
            }
        }

        if (userLocationDot == null)
        {
            Debug.Log("🔍 Searching for UserLocationDot (including inactive)...");
            // GameObject.Find() only finds active objects, so search through all Images
            Image[] allImages = FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Image img in allImages)
            {
                if (img.gameObject.name == "UserLocationDot")
                {
                    userLocationDot = img.gameObject;
                    Debug.Log($"✅ UserLocationDot found! Active: {userLocationDot.activeSelf}");
                    break;
                }
            }

            if (userLocationDot == null)
                Debug.LogError("❌ UserLocationDot not found! Please check scene hierarchy.");
        }

        if (confirmButton == null)
        {
            Debug.Log("🔍 Searching for ConfirmButton (including inactive)...");
            // GameObject.Find() only finds active objects, so search through all Buttons
            Button[] allButtons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Button btn in allButtons)
            {
                if (btn.gameObject.name == "ConfirmButton")
                {
                    confirmButton = btn;
                    Debug.Log($"✅ ConfirmButton found! Active: {btn.gameObject.activeSelf}");
                    break;
                }
            }

            if (confirmButton == null)
                Debug.LogError("❌ ConfirmButton not found! Please check scene hierarchy.");
        }

        if (instructionText == null)
        {
            GameObject textObj = GameObject.Find("InstructionText");
            if (textObj != null)
                instructionText = textObj.GetComponent<Text>();
        }

        Debug.Log("✅ TapToLocateManager references auto-assigned");
    }

    void InitializeUI()
    {
        // Hide confirm button initially
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(false);

        // Set initial instruction
        if (instructionText != null)
            instructionText.text = "Tap your current location on the floor plan";

        // Hide user location dot initially
        if (userLocationDot != null)
            userLocationDot.SetActive(false);

        Debug.Log("TapToLocateManager initialized");
    }
    
    void SetupEventHandlers()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(StartARNavigation);
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void HandleInput()
    {
        // Handle mouse input (for testing in editor)
        if (Input.GetMouseButtonDown(0))
        {
            // Check if click is on a button (not just any UI element)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // Check if it's actually a button or marker
                var pointerData = new UnityEngine.EventSystems.PointerEventData(EventSystem.current);
                pointerData.position = Input.mousePosition;
                var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    // Allow clicks on floor plan image, but block clicks on buttons
                    if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                    {
                        Debug.Log("Click was on button, ignoring for floor plan tap");
                        return;
                    }
                }
            }

            Vector2 inputPosition = Input.mousePosition;
            HandleScreenTap(inputPosition);
        }

        // Handle touch input (for mobile)
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Check if touch is on a button (not just any UI element)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                // Check if it's actually a button or marker
                var pointerData = new UnityEngine.EventSystems.PointerEventData(EventSystem.current);
                pointerData.position = Input.GetTouch(0).position;
                var results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                foreach (var result in results)
                {
                    // Allow touches on floor plan image, but block touches on buttons
                    if (result.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
                    {
                        Debug.Log("Touch was on button, ignoring for floor plan tap");
                        return;
                    }
                }
            }

            Vector2 inputPosition = Input.GetTouch(0).position;
            HandleScreenTap(inputPosition);
        }
    }
    
    void HandleScreenTap(Vector2 screenPosition)
    {
        if (coordinateConverter == null)
        {
            Debug.LogError("CoordinateConverter is not assigned!");
            return;
        }
        
        // Check if tap is on floor plan
        if (!coordinateConverter.IsPositionValid(screenPosition))
        {
            Debug.Log("Tap is outside floor plan area");
            return;
        }
        
        // Convert to world position
        Vector3 worldPos = coordinateConverter.UIToWorldPosition(screenPosition);
        
        if (!locationSelected)
        {
            // First tap - set user location
            SetUserLocation(screenPosition, worldPos);
        }
        else if (!destinationSelected)
        {
            // Second tap - set destination (for now, we'll handle this later)
            Debug.Log($"Destination would be set at: {worldPos}");
        }
    }
    
    void SetUserLocation(Vector2 screenPosition, Vector3 worldPosition)
    {
        selectedWorldPosition = worldPosition;
        locationSelected = true;

        // Show user location dot on UI
        if (userLocationDot != null)
        {
            Debug.Log($"🟢 Setting UserLocationDot active. Current state: {userLocationDot.activeSelf}");
            userLocationDot.SetActive(true);
            Debug.Log($"🟢 UserLocationDot.SetActive(true) called. New state: {userLocationDot.activeSelf}");
            PositionUIElement(userLocationDot, screenPosition);
        }
        else
        {
            Debug.LogError("❌ userLocationDot is NULL!");
        }

        // Update instruction text
        if (instructionText != null)
            instructionText.text = "Location set! Choose a destination room or confirm to start navigation.";

        // Show confirm button
        if (confirmButton != null)
        {
            Debug.Log($"🔘 Setting ConfirmButton active. Current state: {confirmButton.gameObject.activeSelf}");
            confirmButton.gameObject.SetActive(true);
            Debug.Log($"🔘 ConfirmButton.SetActive(true) called. New state: {confirmButton.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError("❌ confirmButton is NULL!");
        }

        if (showDebugInfo)
            Debug.Log($"User location set at world position: {worldPosition}");
    }
    
    void PositionUIElement(GameObject uiElement, Vector2 screenPosition)
    {
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Find canvas directly since TapToLocateManager might not be a child of Canvas
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                rectTransform.position = screenPosition;
                Debug.Log($"📍 Positioned {uiElement.name} at screen position {screenPosition}");
            }
            else
            {
                Debug.LogWarning($"⚠️ Canvas not found or not in ScreenSpaceOverlay mode");
            }
        }
    }
    
    public void SetDestination(Vector3 destinationPos, string destinationName)
    {
        destinationWorldPosition = destinationPos;
        selectedDestinationName = destinationName;
        destinationSelected = true;
        
        if (instructionText != null)
            instructionText.text = $"Destination set: {destinationName}. Ready to start navigation!";
        
        if (showDebugInfo)
            Debug.Log($"Destination set: {destinationName} at {destinationPos}");
    }
    
    public void StartARNavigation()
    {
        Debug.Log("═══════════════════════════════════════════");
        Debug.Log("🚀 START AR NAVIGATION BUTTON CLICKED!");
        Debug.Log("═══════════════════════════════════════════");

        if (!locationSelected)
        {
            Debug.LogWarning("❌ Cannot start: User location not set!");
            return;
        }

        Debug.Log($"✅ User location set: {selectedWorldPosition}");
        Debug.Log($"✅ Destination selected: {destinationSelected}");

        // Store navigation data for next scene
        NavigationData.userStartPosition = selectedWorldPosition;
        NavigationData.destinationPosition = destinationSelected ? destinationWorldPosition : Vector3.zero;
        NavigationData.destinationName = destinationSelected ? selectedDestinationName : "No destination";
        NavigationData.hasValidData = true;

        Debug.Log("📦 Stored navigation data:");
        NavigationData.LogData();

        Debug.Log("🔄 Loading SampleScene...");

        // Load AR scene (SampleScene)
        SceneManager.LoadScene("SampleScene");
    }
    
    void OnValidate()
    {
        // Auto-assign coordinate converter if not set
        if (coordinateConverter == null)
        {
            coordinateConverter = GetComponent<CoordinateConverter>();
        }
    }
}