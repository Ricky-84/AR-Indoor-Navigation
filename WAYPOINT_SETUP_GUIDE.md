# AR Waypoint System Setup Guide for Unity 6
**Unity Version:** 6000.0.58f1
**AR Foundation:** 6.0.6
**Project:** IndoorNavigation

---

## Overview

This guide sets up the **WrightAngle Waypoint System** in your AR scene (SampleScene) to provide real-time navigation guidance after users select their location in the 2D floor plan (FloorPlaneScene).

---

## 📋 Prerequisites Checklist

- ✅ Unity 6 (6000.0.58f1) installed
- ✅ AR Foundation 6.0.6 package installed
- ✅ ARCore 6.0.6 package installed
- ✅ FloorPlaneScene working (2D floor plan system)
- ✅ SampleScene exists (AR scene)

---

## Step 1: Import TextMeshPro Essential Resources

**Unity 6 Note:** TextMeshPro is now integrated into Unity's UGUI package (com.unity.ugui@2.0.0), but resources still need to be imported.

### Instructions:

1. In Unity Editor menu bar, click: **Window → TextMeshPro → Import TMP Essential Resources**
2. Click **Import** in the dialog window
3. Wait for import to complete
4. You should see a new folder: `Assets/TextMesh Pro/`

**Verification:** Check that `Assets/TextMesh Pro/Resources/` exists

---

## Step 2: Create WaypointSettings Asset

### Instructions:

1. **Create the Settings folder (if needed):**
   - Right-click `Assets/Settings/` in Project window
   - If it doesn't exist, create it: Right-click `Assets/` → Create → Folder → Name it "Settings"

2. **Create WaypointSettings ScriptableObject:**
   - Right-click in `Assets/Settings/`
   - Select: **Create → WrightAngle → Waypoint Settings**
   - Name it: `ARNavigationSettings`

3. **Configure the Settings:**
   - Select `ARNavigationSettings` in Project window
   - In Inspector, set these values:

   ```
   Core Functionality:
   ├─ Update Frequency: 0.1
   ├─ Game Mode: Mode3D
   ├─ Marker Prefab: (Leave empty for now - we'll assign this in Step 3)
   ├─ Max Visible Distance: 100
   └─ Ignore Z Axis For Distance 2D: ✓ (checked)

   Off-Screen Indicator:
   ├─ Use Off Screen Indicators: ✓ (checked)
   ├─ Screen Edge Margin: 50
   └─ Flip Off Screen Marker Y: ☐ (unchecked)

   Distance Scaling:
   ├─ Enable Distance Scaling: ✓ (checked)
   ├─ Distance For Default Scale: 10
   ├─ Max Scaling Distance: 50
   ├─ Min Scale Factor: 0.5
   └─ Default Scale Factor: 1.0

   Distance Text (TMPro):
   ├─ Display Distance Text: ✓ (checked)
   ├─ Unit System: Metric
   ├─ Distance Decimal Places: 1
   ├─ Suffix Meters: m
   └─ Suffix Kilometers: km
   ```

4. **Save:** Ctrl+S (or Cmd+S on Mac)

**Verification:** You should see the asset in `Assets/Settings/ARNavigationSettings.asset`

---

## Step 3: Create AR Waypoint Marker Prefab

### Part A: Run the Helper Script

1. **Open any scene** (can be FloorPlaneScene or SampleScene)

2. **Create temporary GameObject:**
   - In Hierarchy, right-click → Create Empty
   - Name it: `MarkerSetupHelper`

3. **Add the helper script:**
   - Select `MarkerSetupHelper` in Hierarchy
   - In Inspector, click **Add Component**
   - Search for: `ARWaypointMarkerSetup`
   - Click to add it

4. **Run the context menu:**
   - In Inspector, find the `ARWaypointMarkerSetup` component
   - Click the ⋮ (three dots) menu on the component
   - Select: **Create AR Waypoint Marker Prefab**

5. **Check the Console:**
   - You should see: "✅ Created AR Waypoint Marker prefab!"
   - And instructions for next steps

6. **Find the created marker:**
   - Look in Hierarchy for a new GameObject: `WaypointMarker`

### Part B: Configure the Marker Components

1. **Select `WaypointMarker` in Hierarchy**

2. **In Inspector, find the `Waypoint Marker UI` component**

3. **Assign the references** (drag from Hierarchy):
   ```
   Waypoint Marker UI (Script):
   ├─ Marker Icon → Drag "MarkerIcon" child GameObject here
   └─ Distance Text Element → Drag "DistanceText" child GameObject here
   ```

   **How to drag:**
   - In Hierarchy, expand `WaypointMarker` to see its children
   - Click and drag `MarkerIcon` from Hierarchy
   - Drop it on the "Marker Icon" field in Inspector
   - Repeat for `DistanceText` → "Distance Text Element" field

4. **Verify the structure in Hierarchy:**
   ```
   WaypointMarker
   ├─ MarkerIcon (Image component - arrow sprite)
   └─ DistanceText (TextMeshProUGUI component)
   ```

5. **Verify assignment in Inspector:**
   - Marker Icon field should show: `MarkerIcon (Image)`
   - Distance Text Element field should show: `DistanceText (TextMeshProUGUI)`

### Part C: Save as Prefab

1. **Create Prefabs folder if needed:**
   - In Project window, right-click `Assets/` → Create → Folder
   - Name it: `Prefabs` (if it doesn't exist)

2. **Save the prefab:**
   - In Hierarchy, **drag** `WaypointMarker`
   - **Drop** it into `Assets/Prefabs/` folder
   - You should see `WaypointMarker.prefab` appear

3. **Delete from Hierarchy:**
   - Select `WaypointMarker` in Hierarchy
   - Press Delete key
   - Also delete `MarkerSetupHelper`

4. **Assign to Settings:**
   - Select `ARNavigationSettings` in Project window
   - In Inspector, find **Marker Prefab** field
   - Drag `Assets/Prefabs/WaypointMarker.prefab` into this field

**Verification:**
- `Assets/Prefabs/WaypointMarker.prefab` exists
- `ARNavigationSettings` has the prefab assigned

---

## Step 4: Setup SampleScene (AR Scene)

### Part A: Open SampleScene

1. In Project window: `Assets/Scenes/SampleScene.unity`
2. Double-click to open

### Part B: Verify AR Setup

Check that your scene has these (from AR Foundation):

```
Hierarchy should contain:
├─ XR Origin (or AR Session Origin)
│  └─ Camera Offset
│     └─ Main Camera (with ARCameraManager, ARCameraBackground)
└─ AR Session (with ARSession component)
```

**If missing:** GameObject → XR → XR Origin (Mobile AR) - this creates both XR Origin and AR Session

### Part C: Create UI Canvas

**Unity 6 Note:** Canvas creation is the same as previous versions.

1. **Right-click in Hierarchy**
2. **UI → Canvas**
3. **Rename** to: `ARNavigationCanvas`

4. **Configure Canvas component:**
   - Render Mode: **Screen Space - Overlay**
   - Pixel Perfect: ☐ (unchecked)
   - Sort Order: 10

5. **Configure Canvas Scaler:**
   - UI Scale Mode: **Scale With Screen Size**
   - Reference Resolution: X=1920, Y=1080
   - Screen Match Mode: **Match Width Or Height**
   - Match: 0.5

**Verification:** You should see a large white rectangle outline in Scene view

### Part D: Create WaypointUIManager

1. **Right-click in Hierarchy → Create Empty**
2. **Rename** to: `WaypointUIManager`

3. **Add the component:**
   - Select `WaypointUIManager` in Hierarchy
   - Click **Add Component** in Inspector
   - Search: `WaypointUIManager` (namespace: WrightAngle.Waypoint)
   - Click to add

4. **Assign references:**
   ```
   Waypoint UI Manager (Script):
   ├─ Settings → Drag "ARNavigationSettings" asset from Assets/Settings/
   ├─ Waypoint Camera → Drag "Main Camera" from Hierarchy (under XR Origin)
   └─ Marker Parent Canvas → Drag "ARNavigationCanvas" from Hierarchy
   ```

**Important:** Make sure you drag the RectTransform of the Canvas, not just any Canvas component.

### Part E: Add ARNavigationBridge

1. **Select `WaypointUIManager` in Hierarchy** (or create new GameObject)

2. **Add component:**
   - Click **Add Component**
   - Search: `ARNavigationBridge`
   - Click to add

3. **Assign references:**
   ```
   AR Navigation Bridge (Script):
   ├─ Ar Camera → Drag "Main Camera" from Hierarchy
   └─ Show Debug Info: ✓ (checked)
   ```

**Verification:** Both components should be on `WaypointUIManager` GameObject

---

## Step 5: Add Waypoint Targets (Sample Destinations)

Now we'll add WaypointTarget components to mark destination locations in your AR scene.

### Instructions:

1. **Create a sample destination:**
   - Right-click in Hierarchy → Create Empty
   - Name: `Destination_RoomA`
   - Position: Set to a location in your building (e.g., `X=10, Y=0, Z=5`)

2. **Add WaypointTarget component:**
   - Select `Destination_RoomA`
   - Click **Add Component**
   - Search: `WaypointTarget` (namespace: WrightAngle.Waypoint)
   - Click to add

3. **Configure:**
   ```
   Waypoint Target (Script):
   ├─ Display Name: Room A
   └─ Activate On Start: ☐ (UNCHECKED - controlled by ARNavigationBridge)
   ```

4. **Add visual marker (optional but helpful):**
   - Right-click `Destination_RoomA` → 3D Object → Cylinder
   - Name it: `VisualMarker`
   - Set Scale: X=0.5, Y=0.05, Z=0.5
   - Set Position: X=0, Y=0, Z=0 (relative to parent)

5. **Repeat for other destinations** you want to track

**Pro Tip:** You can create a prefab of this setup and duplicate it for multiple rooms.

---

## Step 6: Test the System

### Part A: Test in Editor (Play Mode)

1. **Save SampleScene:** Ctrl+S

2. **Open FloorPlaneScene:**
   - File → Open Scene → `Assets/Scenes/FloorPlaneScene.unity`

3. **Play the scene:**
   - Click ▶️ Play button
   - Wait 2 seconds for auto-setup
   - Tap to set your location
   - Click a destination marker
   - Click "Start AR Navigation" button

4. **Scene should switch to SampleScene:**
   - Check Console for logs:
     - "🎯 AR Navigation Bridge - Setting up..."
     - "✅ Created start marker at: [position]"
     - "✅ Created new waypoint target: [name]"
   - Look in Scene view for green cylinder (start) and red cylinder (destination)
   - Waypoint marker UI should appear on screen

### Part B: Test on Android Device

**Requirements:**
- Android device with ARCore support
- USB cable
- Developer options enabled on device

1. **Build Settings:**
   - File → Build Settings
   - Select **Android** platform
   - Click **Switch Platform** if needed

2. **Add scenes:**
   - Click **Add Open Scenes** (or drag both scenes)
   - Ensure order:
     1. FloorPlaneScene
     2. SampleScene

3. **Player Settings:**
   - Click **Player Settings**
   - Verify:
     - Minimum API Level: Android 7.0 'Nougat' (API level 24) or higher
     - Graphics APIs: Remove Vulkan if present, use OpenGLES3
     - ARCore Supported: ✓ (under XR Settings)

4. **Build and Run:**
   - Click **Build and Run**
   - Choose location and filename
   - Wait for build and install
   - App should launch on device

5. **Test flow:**
   - App opens in FloorPlaneScene
   - Tap your location → Select destination → Confirm
   - AR scene loads
   - Move device to detect AR planes
   - Waypoint marker should point to destination

---

## 🐛 Troubleshooting

### Issue: "WaypointSettings not assigned" error
**Solution:** Make sure `ARNavigationSettings` asset is assigned in `WaypointUIManager` component

### Issue: Markers not showing up
**Solution:**
1. Check Console for errors
2. Verify `WaypointMarker.prefab` is assigned in `ARNavigationSettings`
3. Make sure `ARNavigationCanvas` is assigned as Marker Parent Canvas

### Issue: "No navigation data from floor plan scene"
**Solution:**
1. Make sure you completed all steps in FloorPlaneScene before clicking "Start AR Navigation"
2. Check that `NavigationData.hasValidData` is true (add Debug.Log in ARNavigationBridge)

### Issue: TextMeshPro pink text
**Solution:** Import TMP Essential Resources (Step 1)

### Issue: AR Camera not working
**Solution:**
1. Ensure AR Foundation and ARCore packages are installed
2. Check that XR Origin → Main Camera has `ARCameraManager` component
3. Verify AR Session GameObject exists in scene

---

## 📚 Additional Resources

- **WrightAngle Waypoint Docs:** `Assets/WaypointSystem/Docs.pdf`
- **AR Foundation Docs:** [docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@6.0)
- **Unity 6 UI Guide:** [unity.com/resources/scalable-performant-ui-uitoolkit-unity-6](https://unity.com/resources/scalable-performant-ui-uitoolkit-unity-6)

---

## ✅ Success Checklist

After completing all steps, you should have:

- ☐ TextMeshPro Essential Resources imported
- ☐ `ARNavigationSettings.asset` created and configured
- ☐ `WaypointMarker.prefab` created with all components assigned
- ☐ `ARNavigationCanvas` in SampleScene
- ☐ `WaypointUIManager` with all references assigned
- ☐ `ARNavigationBridge` component added
- ☐ At least one `WaypointTarget` in scene
- ☐ Both scenes in Build Settings
- ☐ Tested navigation flow in Editor
- ☐ (Optional) Tested on Android device

---

**Next Steps:** Once everything is working, you can:
1. Add more waypoint targets for all your building locations
2. Customize marker appearance and colors
3. Add pathfinding with Unity NavMesh
4. Implement turn-by-turn navigation instructions
