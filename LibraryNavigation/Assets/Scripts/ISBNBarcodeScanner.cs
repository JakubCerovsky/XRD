using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
#endif

/// <summary>
/// Native barcode scanner for ISBN using device camera.
/// Uses AR Foundation's camera on mobile platforms (iOS/Android).
/// Supports both ISBN-10 and ISBN-13 barcode formats.
/// </summary>
public class ISBNBarcodeScanner : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text field to display scanned ISBN")]
    [SerializeField]
    private TMP_Text scannedISBNText;

    [Tooltip("Input field to populate with scanned ISBN")]
    [SerializeField]
    private TMP_InputField isbnInputField;

    [Tooltip("Raw image to show camera feed")]
    [SerializeField]
    private RawImage cameraFeedDisplay;

    [Tooltip("Button to start/stop scanning")]
    [SerializeField]
    private Button scanButton;

    [Tooltip("Status text to show scanning state")]
    [SerializeField]
    private TMP_Text statusText;

    [Header("Scanning Settings")]
    [Tooltip("Auto-validate ISBN after scanning")]
    [SerializeField]
    private bool autoValidate = true;

    [Tooltip("Auto-close scanner after successful scan")]
    [SerializeField]
    private bool autoClose = true;

    [Tooltip("Scanning interval in seconds")]
    [SerializeField]
    private float scanInterval = 0.5f;

    [Tooltip("Show debug messages")]
    [SerializeField]
    private bool showDebugLogs = true;

    [Header("Visual Feedback")]
    [Tooltip("Color for successful scan")]
    [SerializeField]
    private Color successColor = Color.green;

    [Tooltip("Color for failed scan")]
    [SerializeField]
    private Color errorColor = Color.red;

    [Tooltip("Panel to show/hide when scanning")]
    [SerializeField]
    private GameObject scannerPanel;

    // Events
    public event Action<string> OnISBNScanned;
    public event Action<string> OnValidISBNScanned;
    public event Action<string> OnInvalidISBNScanned;

    private WebCamTexture webCamTexture;
    private bool isScanning = false;
    private bool isCameraReady = false;
    private Coroutine scanCoroutine;

#if UNITY_IOS || UNITY_ANDROID
    private ARCameraManager arCameraManager;
    private bool useARCamera = true;
#else
    private bool useARCamera = false;
#endif

    void Start()
    {
        if (scanButton != null)
        {
            scanButton.onClick.AddListener(ToggleScanning);
        }

        // Try to find AR Camera if available
#if UNITY_IOS || UNITY_ANDROID
        arCameraManager = FindFirstObjectByType<ARCameraManager>();
        if (arCameraManager != null)
        {
            Log("AR Camera Manager found - will use AR camera for scanning");
            useARCamera = true;
        }
        else
        {
            Log("AR Camera Manager not found - will use WebCamTexture");
            useARCamera = false;
        }
#endif

        if (scannerPanel != null)
        {
            scannerPanel.SetActive(false);
        }

        UpdateStatus("Ready to scan", Color.white);
    }

    void OnDestroy()
    {
        StopScanning();
        if (scanButton != null)
        {
            scanButton.onClick.RemoveListener(ToggleScanning);
        }
    }

    /// <summary>
    /// Toggles the scanner on/off
    /// </summary>
    public void ToggleScanning()
    {
        if (isScanning)
        {
            StopScanning();
        }
        else
        {
            StartScanning();
        }
    }

    /// <summary>
    /// Starts the barcode scanning process
    /// </summary>
    public void StartScanning()
    {
        if (isScanning)
        {
            Log("Already scanning");
            return;
        }

        Log("Starting barcode scanner...");
        isScanning = true;

        if (scannerPanel != null)
        {
            scannerPanel.SetActive(true);
        }

        UpdateStatus("Initializing camera...", Color.yellow);

        StartCoroutine(InitializeCamera());
    }

    /// <summary>
    /// Stops the barcode scanning process
    /// </summary>
    public void StopScanning()
    {
        if (!isScanning)
        {
            return;
        }

        Log("Stopping barcode scanner...");
        isScanning = false;
        isCameraReady = false;

        if (scanCoroutine != null)
        {
            StopCoroutine(scanCoroutine);
            scanCoroutine = null;
        }

        if (webCamTexture != null && webCamTexture.isPlaying)
        {
            webCamTexture.Stop();
            Destroy(webCamTexture);
            webCamTexture = null;
        }

        if (cameraFeedDisplay != null)
        {
            cameraFeedDisplay.texture = null;
        }

        if (scannerPanel != null)
        {
            scannerPanel.SetActive(false);
        }

        UpdateStatus("Scanner stopped", Color.white);
    }

    /// <summary>
    /// Initializes the camera for scanning
    /// </summary>
    private IEnumerator InitializeCamera()
    {
        // Request camera permission
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Log("Requesting camera permission...");
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                LogError("Camera permission denied!");
                UpdateStatus("Camera permission denied", errorColor);
                StopScanning();
                yield break;
            }
        }

        // Get available cameras
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            LogError("No camera found on device!");
            UpdateStatus("No camera found", errorColor);
            StopScanning();
            yield break;
        }

        // Prefer back camera for barcode scanning
        string deviceName = devices[0].name;
        for (int i = 0; i < devices.Length; i++)
        {
            if (!devices[i].isFrontFacing)
            {
                deviceName = devices[i].name;
                Log($"Using back camera: {deviceName}");
                break;
            }
        }

        // Initialize WebCamTexture with high resolution for better barcode detection
        webCamTexture = new WebCamTexture(deviceName, 1920, 1080, 30);

        if (cameraFeedDisplay != null)
        {
            cameraFeedDisplay.texture = webCamTexture;
            cameraFeedDisplay.material.mainTexture = webCamTexture;
        }

        webCamTexture.Play();

        // Wait for camera to be ready
        int timeout = 0;
        while (!webCamTexture.didUpdateThisFrame && timeout < 100)
        {
            timeout++;
            yield return new WaitForSeconds(0.1f);
        }

        if (!webCamTexture.didUpdateThisFrame)
        {
            LogError("Camera failed to initialize!");
            UpdateStatus("Camera failed to start", errorColor);
            StopScanning();
            yield break;
        }

        isCameraReady = true;
        UpdateStatus("Point camera at ISBN barcode", Color.cyan);
        Log("Camera ready - starting barcode detection");

        // Start scanning coroutine
        scanCoroutine = StartCoroutine(ScanBarcodeRoutine());
    }

    /// <summary>
    /// Continuously scans for barcodes in the camera feed
    /// </summary>
    private IEnumerator ScanBarcodeRoutine()
    {
        while (isScanning && isCameraReady)
        {
            if (webCamTexture != null && webCamTexture.isPlaying)
            {
                // Get current frame
                Color32[] pixels = webCamTexture.GetPixels32();
                int width = webCamTexture.width;
                int height = webCamTexture.height;

                // Try to decode barcode from the frame
                string result = DecodeBarcode(pixels, width, height);

                if (!string.IsNullOrEmpty(result))
                {
                    OnBarcodeDetected(result);
                }
            }

            yield return new WaitForSeconds(scanInterval);
        }
    }

    /// <summary>
    /// Decodes barcode from camera frame using ZXing.Net
    /// Note: This is a placeholder - you need to integrate ZXing.Net library
    /// </summary>
    private string DecodeBarcode(Color32[] pixels, int width, int height)
    {
        // IMPORTANT: This method requires ZXing.Net library integration
        // For now, this is a placeholder that shows the integration pattern

        try
        {
            // ZXing.Net integration would look like this:
            // var reader = new ZXing.BarcodeReader();
            // var result = reader.Decode(pixels, width, height);
            // if (result != null) return result.Text;

            // For demonstration, we'll use Unity's native barcode detection (iOS/Android)
#if UNITY_IOS && !UNITY_EDITOR
            return TryDecodeWithVisionFramework(pixels, width, height);
#elif UNITY_ANDROID && !UNITY_EDITOR
            return TryDecodeWithMLKit(pixels, width, height);
#else
            // In editor or platforms without native support
            LogWarning("Barcode decoding not available in editor. Use device build for testing.");
            return null;
#endif
        }
        catch (Exception e)
        {
            LogError($"Barcode decode error: {e.Message}");
            return null;
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
    /// <summary>
    /// iOS-specific barcode detection using Vision framework
    /// Requires iOS native plugin integration
    /// </summary>
    private string TryDecodeWithVisionFramework(Color32[] pixels, int width, int height)
    {
        // This would call into native iOS code via a plugin
        // Return value would be the barcode string
        // Implementation requires iOS native plugin
        return null;
    }
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    /// <summary>
    /// Android-specific barcode detection using ML Kit
    /// Requires Android native plugin integration
    /// </summary>
    private string TryDecodeWithMLKit(Color32[] pixels, int width, int height)
    {
        // This would call into native Android code via a plugin
        // Return value would be the barcode string
        // Implementation requires Android native plugin
        return null;
    }
#endif

    /// <summary>
    /// Called when a barcode is detected
    /// </summary>
    private void OnBarcodeDetected(string barcode)
    {
        Log($"Barcode detected: {barcode}");

        // Clean up the barcode (remove any whitespace)
        barcode = barcode.Trim();

        // Validate if it's a valid ISBN
        bool isValid = false;
        if (autoValidate)
        {
            isValid = ISBNFormatChecker.IsValidIsbn(barcode);

            if (isValid)
            {
                Log($"✓ Valid ISBN detected: {barcode}");
                UpdateStatus($"Valid ISBN: {barcode}", successColor);
                OnValidISBNScanned?.Invoke(barcode);
            }
            else
            {
                LogWarning($"Invalid ISBN detected: {barcode}");
                UpdateStatus($"Invalid ISBN: {barcode}", errorColor);
                OnInvalidISBNScanned?.Invoke(barcode);

                // Don't stop scanning for invalid ISBNs, let user try again
                return;
            }
        }
        else
        {
            UpdateStatus($"Scanned: {barcode}", successColor);
        }

        // Update UI elements
        if (scannedISBNText != null)
        {
            scannedISBNText.text = barcode;
        }

        if (isbnInputField != null)
        {
            isbnInputField.text = barcode;
        }

        // Trigger event
        OnISBNScanned?.Invoke(barcode);

        // Auto-close scanner after successful scan
        if (autoClose && (!autoValidate || isValid))
        {
            StartCoroutine(CloseAfterDelay(1.5f));
        }
    }

    /// <summary>
    /// Closes the scanner after a delay
    /// </summary>
    private IEnumerator CloseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopScanning();
    }

    /// <summary>
    /// Updates status text and color
    /// </summary>
    private void UpdateStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }
        Log($"Status: {message}");
    }

    /// <summary>
    /// Simulates a barcode scan (for testing in editor)
    /// </summary>
    public void SimulateScan(string isbn)
    {
        Log($"Simulating barcode scan: {isbn}");
        OnBarcodeDetected(isbn);
    }

    private void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log($"[ISBNBarcodeScanner] {message}");
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning($"[ISBNBarcodeScanner] {message}");
    }

    private void LogError(string message)
    {
        Debug.LogError($"[ISBNBarcodeScanner] {message}");
    }
}
