using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script to create waypoint marker prefab for AR navigation
/// Run this once to create the marker prefab, then delete this script
/// </summary>
public class ARWaypointMarkerSetup : MonoBehaviour
{
    [ContextMenu("Create AR Waypoint Marker Prefab")]
    public void CreateWaypointMarkerPrefab()
    {
        // Create root GameObject for marker
        GameObject markerRoot = new GameObject("WaypointMarker");

        // Add RectTransform
        RectTransform rootRect = markerRoot.AddComponent<RectTransform>();
        rootRect.sizeDelta = new Vector2(100, 120);
        rootRect.anchorMin = new Vector2(0.5f, 0.5f);
        rootRect.anchorMax = new Vector2(0.5f, 0.5f);
        rootRect.pivot = new Vector2(0.5f, 0.5f);

        // Create icon image (main visual element)
        GameObject iconObj = new GameObject("MarkerIcon");
        iconObj.transform.SetParent(markerRoot.transform, false);
        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(80, 80);
        iconRect.anchoredPosition = new Vector2(0, 10);

        Image iconImage = iconObj.AddComponent<Image>();
        iconImage.color = new Color(0.2f, 0.8f, 1f, 0.9f); // Light blue
        iconImage.sprite = CreateArrowSprite(); // Use arrow pointing up

        // Create distance text (using TextMeshPro)
        GameObject textObj = new GameObject("DistanceText");
        textObj.transform.SetParent(markerRoot.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(100, 30);
        textRect.anchoredPosition = new Vector2(0, -40);

        TextMeshProUGUI distanceText = textObj.AddComponent<TextMeshProUGUI>();
        distanceText.text = "0m";
        distanceText.fontSize = 18;
        distanceText.color = Color.white;
        distanceText.alignment = TextAlignmentOptions.Center;
        distanceText.fontStyle = FontStyles.Bold;

        // Add outline for better visibility
        distanceText.outlineWidth = 0.3f;
        distanceText.outlineColor = Color.black;

        // Add WaypointMarkerUI component (from WrightAngle system)
        // IMPORTANT: Must be added AFTER creating the child objects
        var markerUI = markerRoot.AddComponent<WrightAngle.Waypoint.WaypointMarkerUI>();

        Debug.Log("✅ Created AR Waypoint Marker prefab!");
        Debug.Log("📋 Next steps:");
        Debug.Log("   1. Select 'WaypointMarker' in Hierarchy");
        Debug.Log("   2. In Inspector, find 'Waypoint Marker UI' component");
        Debug.Log("   3. Assign references:");
        Debug.Log("      - Marker Icon → Drag 'MarkerIcon' child");
        Debug.Log("      - Distance Text Element → Drag 'DistanceText' child");
        Debug.Log("   4. Drag WaypointMarker to Assets/Prefabs/ to save as prefab");
        Debug.Log("   5. Delete WaypointMarker from Hierarchy");
        Debug.Log("   6. Assign the prefab to ARNavigationSettings asset");
    }

    Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f - 2;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    Sprite CreateArrowSprite()
    {
        int size = 64;
        Texture2D texture = new Texture2D(size, size);
        Color[] pixels = new Color[size * size];

        // Simple triangle pointing up
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Triangle calculation
                float normalizedX = (x - size / 2f) / (size / 2f);
                float normalizedY = y / (float)size;

                if (normalizedY > 0.3f && Mathf.Abs(normalizedX) < (1.0f - normalizedY) * 0.7f)
                {
                    pixels[y * size + x] = Color.white;
                }
                else
                {
                    pixels[y * size + x] = Color.clear;
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }
}
