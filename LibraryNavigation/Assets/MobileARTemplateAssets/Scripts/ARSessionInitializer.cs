using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Ensures AR Session and AR Camera Background are properly initialized.
/// This is critical for the camera feed to display correctly.
/// Attach this to any GameObject in your scene (or the AR Session GameObject itself).
/// </summary>
public class ARSessionInitializer : MonoBehaviour
{
    [Header("AR Components (Auto-detected if not assigned)")]
    [Tooltip("Reference to the ARSession component")]
    [SerializeField]
    private ARSession arSession;

    [Tooltip("Reference to the ARCameraManager component")]
    [SerializeField]
    private ARCameraManager arCameraManager;

    [Tooltip("Reference to the ARCameraBackground component")]
    [SerializeField]
    private ARCameraBackground arCameraBackground;

    [Header("Debug")]
    [Tooltip("Show debug logs in console")]
    [SerializeField]
    private bool showDebugLogs = true;

    void Start()
    {
        InitializeARComponents();
    }

    /// <summary>
    /// Finds and initializes all required AR components.
    /// </summary>
    void InitializeARComponents()
    {
        Log("========== AR SESSION INITIALIZER ==========");

        // Auto-find ARSession if not assigned
        if (arSession == null)
        {
            arSession = FindFirstObjectByType<ARSession>();
            if (arSession == null)
            {
                LogError("✗ ARSession not found! Camera feed will not work.");
                LogError("Solution: Right-click Hierarchy > XR > Device-based > AR Session");
                return;
            }
        }
        Log($"✓ ARSession found: {arSession.gameObject.name}");

        // Ensure ARSession is enabled
        if (!arSession.enabled)
        {
            arSession.enabled = true;
            Log("  → Enabled ARSession component");
        }

        // Auto-find ARCameraManager if not assigned
        if (arCameraManager == null)
        {
            arCameraManager = FindFirstObjectByType<ARCameraManager>();
            if (arCameraManager == null)
            {
                LogError("✗ ARCameraManager not found! Camera feed will not work.");
                LogError("Solution: Add ARCameraManager component to your Main Camera");
                return;
            }
        }
        Log($"✓ ARCameraManager found: {arCameraManager.gameObject.name}");

        // Ensure ARCameraManager is enabled
        if (!arCameraManager.enabled)
        {
            arCameraManager.enabled = true;
            Log("  → Enabled ARCameraManager component");
        }

        // Auto-find ARCameraBackground if not assigned
        if (arCameraBackground == null)
        {
            arCameraBackground = FindFirstObjectByType<ARCameraBackground>();
            if (arCameraBackground == null)
            {
                LogError("✗ ARCameraBackground not found! Camera feed will NOT display (black screen).");
                LogError("Solution: Add ARCameraBackground component to your Main Camera");
                return;
            }
        }
        Log($"✓ ARCameraBackground found: {arCameraBackground.gameObject.name}");

        // Ensure ARCameraBackground is enabled - THIS IS CRITICAL!
        if (!arCameraBackground.enabled)
        {
            arCameraBackground.enabled = true;
            Log("  → Enabled ARCameraBackground component (this fixes black screen!)");
        }

        // Verify the camera background will render
        if (arCameraBackground.enabled && arCameraManager.enabled && arSession.enabled)
        {
            Log("✓✓✓ All AR components initialized successfully!");
            Log("Camera feed should now be visible.");
        }
        else
        {
            LogWarning("⚠ Some AR components are disabled. Camera feed may not work.");
        }

        Log("==========================================");
    }

    void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log($"[ARSessionInitializer] {message}");
    }

    void LogWarning(string message)
    {
        Debug.LogWarning($"[ARSessionInitializer] {message}");
    }

    void LogError(string message)
    {
        Debug.LogError($"[ARSessionInitializer] {message}");
    }
}
