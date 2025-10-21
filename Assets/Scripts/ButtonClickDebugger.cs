using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Attach this to the "Start AR Navigation" button to debug click issues
/// </summary>
public class ButtonClickDebugger : MonoBehaviour, IPointerClickHandler
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        Debug.Log("═══════════════════════════════════");
        Debug.Log("🔍 BUTTON DEBUGGER INITIALIZED");
        Debug.Log($"Button: {gameObject.name}");
        Debug.Log($"Button enabled: {button.enabled}");
        Debug.Log($"Button interactable: {button.interactable}");
        Debug.Log($"Persistent listeners: {button.onClick.GetPersistentEventCount()}");

        for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
        {
            Debug.Log($"  Listener {i}: {button.onClick.GetPersistentTarget(i)?.GetType().Name}.{button.onClick.GetPersistentMethodName(i)}");
        }

        Debug.Log("═══════════════════════════════════");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("═══════════════════════════════════");
        Debug.Log("🖱️ BUTTON CLICKED!");
        Debug.Log($"Button: {gameObject.name}");
        Debug.Log($"Event position: {eventData.position}");
        Debug.Log($"Event will call {button.onClick.GetPersistentEventCount()} persistent listeners");
        Debug.Log("═══════════════════════════════════");
    }

    void Update()
    {
        // Check if something is blocking the button
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            // Check what's under the mouse
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = mousePos;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log($"🎯 Click detected at {mousePos}");
                Debug.Log($"   UI elements hit (top to bottom):");
                foreach (var result in results)
                {
                    Debug.Log($"   - {result.gameObject.name} (depth: {result.depth})");

                    // Check if our button is in the list
                    if (result.gameObject == gameObject)
                    {
                        Debug.Log($"   ✅ BUTTON WAS HIT!");
                    }
                }
            }
        }
    }
}
