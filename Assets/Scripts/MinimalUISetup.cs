using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Minimal UI setup for floor plan navigation
/// Run this once to create the basic Canvas structure
/// </summary>
public class MinimalUISetup : MonoBehaviour
{
    [ContextMenu("Setup UI")]
    public void SetupUI()
    {
        Debug.Log("🎨 Setting up minimal UI...");

        // Create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Main Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            Debug.Log("✅ Created Canvas");
        }

        // Create EventSystem if missing
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<EventSystem>();
            eventSystemObj.AddComponent<StandaloneInputModule>();

            Debug.Log("✅ Created EventSystem");
        }

        // Create Floor Plan RawImage
        RawImage floorPlan = FindFirstObjectByType<RawImage>();
        if (floorPlan == null)
        {
            GameObject floorPlanObj = new GameObject("FloorPlanImage");
            floorPlanObj.transform.SetParent(canvas.transform, false);

            floorPlan = floorPlanObj.AddComponent<RawImage>();
            RectTransform rect = floorPlan.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, 50);
            rect.sizeDelta = new Vector2(900, 400);

            Debug.Log("✅ Created Floor Plan Image");
        }

        // Create User Location Dot
        if (GameObject.Find("UserLocationDot") == null)
        {
            GameObject dotObj = new GameObject("UserLocationDot");
            dotObj.transform.SetParent(canvas.transform, false);

            Image dot = dotObj.AddComponent<Image>();
            dot.color = Color.green;

            RectTransform dotRect = dot.rectTransform;
            dotRect.sizeDelta = new Vector2(20, 20);
            dotRect.anchorMin = new Vector2(0.5f, 0.5f);
            dotRect.anchorMax = new Vector2(0.5f, 0.5f);
            dotRect.pivot = new Vector2(0.5f, 0.5f);

            dotObj.SetActive(false);

            Debug.Log("✅ Created User Location Dot");
        }

        // Create Instruction Text
        if (GameObject.Find("InstructionText") == null)
        {
            GameObject textObj = new GameObject("InstructionText");
            textObj.transform.SetParent(canvas.transform, false);

            Text text = textObj.AddComponent<Text>();
            text.text = "Tap your current location on the floor plan";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 24;
            text.color = Color.black;
            text.alignment = TextAnchor.MiddleCenter;

            RectTransform textRect = text.rectTransform;
            textRect.anchorMin = new Vector2(0.5f, 1);
            textRect.anchorMax = new Vector2(0.5f, 1);
            textRect.pivot = new Vector2(0.5f, 1);
            textRect.anchoredPosition = new Vector2(0, -20);
            textRect.sizeDelta = new Vector2(800, 50);

            Debug.Log("✅ Created Instruction Text");
        }

        // Create Confirm Button
        if (GameObject.Find("ConfirmButton") == null)
        {
            GameObject buttonObj = new GameObject("ConfirmButton");
            buttonObj.transform.SetParent(canvas.transform, false);

            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.8f, 0.2f);

            Button button = buttonObj.AddComponent<Button>();

            RectTransform buttonRect = buttonImage.rectTransform;
            buttonRect.anchorMin = new Vector2(0.5f, 0);
            buttonRect.anchorMax = new Vector2(0.5f, 0);
            buttonRect.pivot = new Vector2(0.5f, 0);
            buttonRect.anchoredPosition = new Vector2(0, 50);
            buttonRect.sizeDelta = new Vector2(400, 80);

            // Button text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            Text buttonText = textObj.AddComponent<Text>();
            buttonText.text = "Start AR Navigation";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonText.fontSize = 28;
            buttonText.color = Color.white;
            buttonText.alignment = TextAnchor.MiddleCenter;

            RectTransform buttonTextRect = buttonText.rectTransform;
            buttonTextRect.anchorMin = Vector2.zero;
            buttonTextRect.anchorMax = Vector2.one;
            buttonTextRect.offsetMin = Vector2.zero;
            buttonTextRect.offsetMax = Vector2.zero;

            buttonObj.SetActive(false);

            Debug.Log("✅ Created Confirm Button");
        }

        // Create TapToLocateManager
        TapToLocateManager tapManager = FindFirstObjectByType<TapToLocateManager>();
        if (tapManager == null)
        {
            GameObject managerObj = new GameObject("TapToLocateManager");
            tapManager = managerObj.AddComponent<TapToLocateManager>();

            Debug.Log("✅ Created TapToLocateManager");
        }

        // Create CoordinateConverter
        CoordinateConverter converter = FindFirstObjectByType<CoordinateConverter>();
        if (converter == null)
        {
            GameObject converterObj = new GameObject("CoordinateConverter");
            converter = converterObj.AddComponent<CoordinateConverter>();

            Debug.Log("✅ Created CoordinateConverter");
        }

        Debug.Log("🎉 Minimal UI setup complete!");
        Debug.Log("💡 Now DirectFloorPlanLoader can create markers on the floor plan");
    }
}
