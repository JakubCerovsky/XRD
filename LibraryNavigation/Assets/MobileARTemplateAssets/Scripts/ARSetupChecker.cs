using UnityEngine;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// Debug script to check if all required AR components are present in the scene.
/// Attach this to any GameObject and check the Console on Start.
/// </summary>
public class ARSetupChecker : MonoBehaviour
{
    void Start()
    {
        Debug.Log("========== AR SETUP CHECKER ==========");
        
        // Check for AR Session
        ARSession session = FindFirstObjectByType<ARSession>();
        if (session != null)
            Debug.Log("✓ AR Session found: " + session.gameObject.name);
        else
            Debug.LogError("✗ AR Session NOT FOUND! Add 'XR > AR Session' to your scene.");
        
        // Check for AR Camera Manager
        ARCameraManager cameraManager = FindFirstObjectByType<ARCameraManager>();
        if (cameraManager != null)
            Debug.Log("✓ AR Camera Manager found: " + cameraManager.gameObject.name);
        else
            Debug.LogError("✗ AR Camera Manager NOT FOUND! Add it to your Main Camera.");
        
        // Check for AR Camera Background
        ARCameraBackground cameraBackground = FindFirstObjectByType<ARCameraBackground>();
        if (cameraBackground != null)
            Debug.Log("✓ AR Camera Background found: " + cameraBackground.gameObject.name);
        else
            Debug.LogError("✗ AR Camera Background NOT FOUND! Add it to your Main Camera.");
        
        // Check for Camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            Debug.Log("✓ Main Camera found: " + mainCamera.gameObject.name);
        else
            Debug.LogError("✗ Main Camera NOT FOUND!");
        
        // Check for XR Origin
        var xrOrigin = FindFirstObjectByType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
            Debug.Log("✓ XR Origin found: " + xrOrigin.gameObject.name);
        else
            Debug.LogWarning("⚠ XR Origin not found (may not be critical if using AR Session Origin)");
        
        // Check for AR Tracked Image Manager
        ARTrackedImageManager trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();
        if (trackedImageManager != null)
            Debug.Log("✓ AR Tracked Image Manager found: " + trackedImageManager.gameObject.name);
        else
            Debug.LogWarning("⚠ AR Tracked Image Manager not found (needed for image tracking)");
        
        Debug.Log("====================================");
        Debug.Log("If you see ✗ errors above, the camera won't work!");
        Debug.Log("To fix: Right-click in Hierarchy > XR > Device-based > AR Session");
        Debug.Log("and Right-click in Hierarchy > XR > Device-based > XR Origin (Mobile AR)");
    }
}
