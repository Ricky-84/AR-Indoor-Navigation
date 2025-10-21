# AR Navigation Bridge Setup Guide

## Current Status
✅ FloorPlaneScene is fully working - green dot and navigation button appear correctly
❌ SampleScene is missing the ARNavigationBridge component to receive navigation data

## What You Need to Do

### Step 1: Add ARNavigationBridge to SampleScene

1. **Open SampleScene** in Unity (not FloorPlaneScene)
   - In Project window: Assets/Scenes/SampleScene.unity
   - Double-click to open

2. **Create new GameObject for AR Bridge**
   - Right-click in Hierarchy → Create Empty
   - Rename it to: `ARNavigationBridge`

3. **Add the ARNavigationBridge script**
   - Select the new `ARNavigationBridge` GameObject
   - In Inspector, click "Add Component"
   - Search for: `ARNavigationBridge`
   - Click to add it

4. **Configure the script (optional)**
   - In Inspector, you'll see:
     - **AR Camera**: Leave empty (script auto-finds it)
     - **Show Debug Info**: Check this box (✓) to see debug logs

5. **Save the scene**
   - Ctrl+S or File → Save

### Step 2: Test the Complete Navigation Flow

1. **Start in FloorPlaneScene** (Play mode)
   - Click on the floor plan to set your location (green dot appears)
   - Click a red marker to select destination (e.g., C104)
   - Click "Start AR Navigation" button

2. **Watch the console for these messages:**
   ```
   🎯 AR Navigation Bridge - Setting up...
   NavigationData Status:
     - Has Valid Data: True
     - User Start Position: (x, y, z)
     - Destination Position: (x, y, z)
     - Destination Name: C104
   ✅ Created start marker at: (x, y, z)
   ✅ Found existing waypoint target: C104
   ✅ Moved XR Origin to start position: (x, y, z)
   ```

3. **In SampleScene (AR view), you should see:**
   - Green cylinder marker at your start position
   - Red cylinder marker at destination (C104)
   - Waypoint UI marker showing "C104" with direction arrow
   - AR camera positioned at your selected start location

### Step 3: Verify Waypoint System Integration

The `ARNavigationBridge` integrates with the WaypointSystem asset:
- **WaypointUIManager** (already in scene) shows on-screen markers pointing to destinations
- **WaypointTarget** components (on each Waypoint_ object) mark trackable locations
- When you select C104 in FloorPlaneScene, it activates the corresponding Waypoint_C104 in AR

## How It Works

**FloorPlaneScene → SampleScene Data Flow:**
1. User clicks floor plan → `TapToLocateManager` converts screen tap to 3D world position
2. User clicks destination marker → stores destination name and position
3. User clicks "Start AR Navigation" → stores data in `NavigationData` static class
4. `SceneManager.LoadScene("SampleScene")` transitions to AR
5. **ARNavigationBridge.Start()** runs in SampleScene
6. Reads `NavigationData.userStartPosition` and `NavigationData.destinationPosition`
7. Creates visual markers (green start, red destination)
8. Moves XR Origin to start position
9. Activates WaypointTarget for destination → WaypointUIManager shows AR navigation UI

## Troubleshooting

**If you see "⚠️ No navigation data from floor plan scene!"**
- The NavigationData wasn't set properly in FloorPlaneScene
- Check that you clicked both a location AND a destination before clicking the button

**If you see "⚠️ XR Origin not found"**
- The script will fall back to moving the Main Camera directly
- Check that SampleScene has "XR Origin (Mobile AR)" in hierarchy

**If waypoint UI markers don't appear:**
- Check that WaypointUIManager is active in SampleScene hierarchy
- Verify it has a Canvas reference assigned in Inspector

## Expected Coordinate System

The 3D world coordinates should match between scenes:
- **Building bounds**: 300m x 140m (as set in WaypointManager3D)
- **Origin**: Center of building at Y=0
- **Example positions**:
  - C104 classroom: approximately (41.40, 0.00, 20.88)
  - User start position: varies based on floor plan click (e.g., 92.83, 0.00, 10.28)

## Next Steps After Setup

Once ARNavigationBridge is added and working:
1. Test on Android device with ARCore
2. Fine-tune AR camera positioning (may need Y-offset adjustment)
3. Add NavMesh path visualization (optional - show line from start to destination)
4. Add turn-by-turn navigation instructions (optional)
5. Add distance/progress tracking (optional)
