using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Minimal AR manager for the Library Navigation app.
/// Handles AR plane detection and ensures camera feed is visible.
/// This replaces the functionality from deleted ARTemplateMenuManager/GoalManager scripts.
/// Attach this to your UI GameObject.
/// </summary>
public class ARLibraryNavigationManager : MonoBehaviour
{
    [Header("AR Components (Auto-detected if not assigned)")]
    [Tooltip("AR Session component - initializes the AR session")]
    [SerializeField]
    private ARSession arSession;

    [Tooltip("AR Camera Manager - manages the AR camera")]
    [SerializeField]
    private ARCameraManager arCameraManager;

    [Tooltip("AR Camera Background - renders the camera feed")]
    [SerializeField]
    private ARCameraBackground arCameraBackground;

    [Tooltip("AR Plane Manager - detects AR planes/surfaces")]
    [SerializeField]
    private ARPlaneManager arPlaneManager;

    [Header("AR Settings")]
    [Tooltip("Should AR planes be visible? (Set to false for cleaner library navigation view)")]
    [SerializeField]
    private bool showARPlanes = false;

    [Tooltip("Enable plane detection at startup")]
    [SerializeField]
    private bool enablePlaneDetection = true;

    [Header("Debug")]
    [Tooltip("Show debug logs")]
    [SerializeField]
    private bool showDebugLogs = true;

    void Start()
    {
        InitializeAR();
    }

    /// <summary>
    /// Initializes all AR components and ensures camera feed is visible.
    /// </summary>
    void InitializeAR()
    {
        Log("========== AR Library Navigation Manager ==========");

        // Auto-find AR Session
        if (arSession == null)
        {
            arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                LogError("✗ ARSession not found! AR will not work.");
                return;
            }
        }
        
        // Ensure AR Session is enabled
        if (!arSession.enabled)
        {
            arSession.enabled = true;
            Log("✓ Enabled ARSession");
        }
        Log($"✓ ARSession initialized: {arSession.gameObject.name}");

        // Auto-find AR Camera Manager
        if (arCameraManager == null)
        {
            arCameraManager = FindFirstObjectByType<ARCameraManager>();
            if (arCameraManager == null)
            {
                LogError("✗ ARCameraManager not found! Camera feed will not work.");
                return;
            }
        }

        // Ensure AR Camera Manager is enabled
        if (!arCameraManager.enabled)
        {
            arCameraManager.enabled = true;
            Log("✓ Enabled ARCameraManager");
        }
        Log($"✓ ARCameraManager initialized: {arCameraManager.gameObject.name}");

        // Auto-find AR Camera Background (CRITICAL for camera feed)
        if (arCameraBackground == null)
        {
            arCameraBackground = FindFirstObjectByType<ARCameraBackground>();
            if (arCameraBackground == null)
            {
                LogError("✗ ARCameraBackground not found! You will see BLACK SCREEN.");
                LogError("Add ARCameraBackground component to your Main Camera!");
                return;
            }
        }

        // Ensure AR Camera Background is enabled (THIS SHOWS THE CAMERA FEED!)
        if (!arCameraBackground.enabled)
        {
            arCameraBackground.enabled = true;
            Log("✓ Enabled ARCameraBackground - camera feed should now be visible!");
        }
        Log($"✓ ARCameraBackground initialized: {arCameraBackground.gameObject.name}");

        // Auto-find AR Plane Manager
        if (arPlaneManager == null)
        {
            arPlaneManager = FindFirstObjectByType<ARPlaneManager>();
            if (arPlaneManager != null)
            {
                Log($"✓ ARPlaneManager found: {arPlaneManager.gameObject.name}");
            }
            else
            {
                LogWarning("⚠ ARPlaneManager not found. Plane detection will not work (may not be needed for library navigation).");
            }
        }

        // Configure plane detection
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = enablePlaneDetection;
            
            if (enablePlaneDetection)
            {
                Log($"✓ Plane detection enabled (planes visible: {showARPlanes})");
                
                // Subscribe to plane changes to control visibility
                arPlaneManager.trackablesChanged.AddListener(OnPlanesChanged);
            }
            else
            {
                Log("Plane detection disabled");
            }
        }

        Log("========== AR Initialization Complete ==========");
        Log("Camera feed should now be visible on the device.");
    }

    void OnDestroy()
    {
        // Unsubscribe from plane changes
        if (arPlaneManager != null)
        {
            arPlaneManager.trackablesChanged.RemoveListener(OnPlanesChanged);
        }
    }

    /// <summary>
    /// Called when AR planes are added, updated, or removed.
    /// Controls plane visibility based on settings.
    /// </summary>
    void OnPlanesChanged(ARTrackablesChangedEventArgs<ARPlane> changes)
    {
        // Control visibility of newly detected planes
        foreach (var plane in changes.added)
        {
            SetPlaneVisibility(plane, showARPlanes);
        }

        foreach (var plane in changes.updated)
        {
            SetPlaneVisibility(plane, showARPlanes);
        }

        if (changes.added.Count > 0 || changes.updated.Count > 0)
        {
            Log($"Planes detected: {changes.added.Count} new, {changes.updated.Count} updated");
        }
    }

    /// <summary>
    /// Sets the visibility of an AR plane.
    /// </summary>
    void SetPlaneVisibility(ARPlane plane, bool visible)
    {
        if (plane == null) return;

        // Find the plane's visual components and set their visibility
        var meshRenderer = plane.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visible;
        }

        var lineRenderer = plane.GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = visible;
        }

        // Also check for ARFeatheredPlaneMeshVisualizerCompanion (if it exists in your project)
        var visualizerComponent = plane.gameObject.GetComponent("ARFeatheredPlaneMeshVisualizerCompanion");
        if (visualizerComponent != null)
        {
            // Use reflection to set visualizeSurfaces property
            var visualizerType = visualizerComponent.GetType();
            var property = visualizerType.GetProperty("visualizeSurfaces");
            if (property != null && property.CanWrite)
            {
                property.SetValue(visualizerComponent, visible);
            }
        }
    }

    /// <summary>
    /// Toggles AR plane visibility on/off.
    /// Can be called from UI buttons if needed.
    /// </summary>
    public void TogglePlaneVisibility()
    {
        showARPlanes = !showARPlanes;
        Log($"Plane visibility toggled: {showARPlanes}");

        // Update all existing planes
        if (arPlaneManager != null)
        {
            foreach (var plane in arPlaneManager.trackables)
            {
                SetPlaneVisibility(plane, showARPlanes);
            }
        }
    }

    /// <summary>
    /// Enables AR plane detection.
    /// </summary>
    public void EnablePlaneDetection()
    {
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = true;
            enablePlaneDetection = true;
            Log("✓ Plane detection enabled");
        }
    }

    /// <summary>
    /// Disables AR plane detection.
    /// </summary>
    public void DisablePlaneDetection()
    {
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = false;
            enablePlaneDetection = false;
            Log("Plane detection disabled");
        }
    }

    void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log($"[ARLibraryNavigation] {message}");
    }

    void LogWarning(string message)
    {
        Debug.LogWarning($"[ARLibraryNavigation] {message}");
    }

    void LogError(string message)
    {
        Debug.LogError($"[ARLibraryNavigation] {message}");
    }
}
