using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Integration helper to connect barcode scanner with LibraryNavigationUIManager.
/// Add this to the same GameObject as your LibraryNavigationUIManager or reference it.
/// </summary>
public class BarcodeScannerIntegration : MonoBehaviour
{
    [Header("Scanner Reference")]
    [Tooltip("Reference to the barcode scanner component")]
    [SerializeField]
    private ISBNBarcodeZXingScanner barcodeScanner;

    [Header("UI Manager Reference")]
    [Tooltip("Reference to the LibraryNavigationUIManager")]
    [SerializeField]
    private LibraryNavigationUIManager uiManager;

    [Header("UI Elements")]
    [Tooltip("ISBN input field to populate")]
    [SerializeField]
    private TMP_InputField isbnInputField;

    [Tooltip("Button to trigger barcode scanning")]
    [SerializeField]
    private Button scanBarcodeButton;

    [Tooltip("Alternative scan button in the book search panel")]
    [SerializeField]
    private Button bookSearchScanButton;

    [Header("Settings")]
    [Tooltip("Automatically start navigation after valid scan")]
    [SerializeField]
    private bool autoStartNavigation = false;

    [Tooltip("Show feedback messages")]
    [SerializeField]
    private bool showFeedback = true;

    void Start()
    {
        // Auto-find components if not assigned
        if (barcodeScanner == null)
        {
            barcodeScanner = FindFirstObjectByType<ISBNBarcodeZXingScanner>();
            if (barcodeScanner == null)
            {
                Debug.LogError("[BarcodeScannerIntegration] ISBNBarcodeZXingScanner not found! Please add it to the scene.");
                return;
            }
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<LibraryNavigationUIManager>();
        }

        // Wire up button events
        if (scanBarcodeButton != null)
        {
            scanBarcodeButton.onClick.AddListener(OnScanButtonClicked);
        }

        if (bookSearchScanButton != null)
        {
            bookSearchScanButton.onClick.AddListener(OnScanButtonClicked);
        }

        // Subscribe to scanner events
        if (barcodeScanner != null)
        {
            barcodeScanner.OnValidISBNScanned += OnValidISBNScanned;
            barcodeScanner.OnInvalidISBNScanned += OnInvalidISBNScanned;
            barcodeScanner.OnISBNScanned += OnISBNScanned;
        }
    }

    void OnDestroy()
    {
        // Clean up event listeners
        if (scanBarcodeButton != null)
        {
            scanBarcodeButton.onClick.RemoveListener(OnScanButtonClicked);
        }

        if (bookSearchScanButton != null)
        {
            bookSearchScanButton.onClick.RemoveListener(OnScanButtonClicked);
        }

        if (barcodeScanner != null)
        {
            barcodeScanner.OnValidISBNScanned -= OnValidISBNScanned;
            barcodeScanner.OnInvalidISBNScanned -= OnInvalidISBNScanned;
            barcodeScanner.OnISBNScanned -= OnISBNScanned;
        }
    }

    /// <summary>
    /// Called when scan button is clicked
    /// </summary>
    private void OnScanButtonClicked()
    {
        if (barcodeScanner != null)
        {
            barcodeScanner.StartScanning();
        }
        else
        {
            Debug.LogError("[BarcodeScannerIntegration] Barcode scanner not assigned!");
        }
    }

    /// <summary>
    /// Called when any ISBN is scanned
    /// </summary>
    private void OnISBNScanned(string isbn)
    {
        Debug.Log($"[BarcodeScannerIntegration] ISBN scanned: {isbn}");

        // Populate input field
        if (isbnInputField != null)
        {
            isbnInputField.text = isbn;
        }
    }

    /// <summary>
    /// Called when a valid ISBN is scanned
    /// </summary>
    private void OnValidISBNScanned(string isbn)
    {
        Debug.Log($"[BarcodeScannerIntegration] Valid ISBN scanned: {isbn}");

        // Populate input field
        if (isbnInputField != null)
        {
            isbnInputField.text = isbn;
        }

        // Show success feedback
        if (showFeedback)
        {
            ShowFeedback($"✓ Valid ISBN: {isbn}", Color.green);
        }

        // Auto-start navigation if enabled
        if (autoStartNavigation && uiManager != null)
        {
            // Use reflection to call private method or make it public
            // For now, just log it
            Debug.Log($"[BarcodeScannerIntegration] Would start navigation to ISBN: {isbn}");
            // uiManager.StartNavigation(isbn); // If you make this method public
        }
    }

    /// <summary>
    /// Called when an invalid ISBN is scanned
    /// </summary>
    private void OnInvalidISBNScanned(string isbn)
    {
        Debug.LogWarning($"[BarcodeScannerIntegration] Invalid ISBN scanned: {isbn}");

        // Show error feedback
        if (showFeedback)
        {
            ShowFeedback($"✗ Invalid ISBN: {isbn}", Color.red);
        }
    }

    /// <summary>
    /// Shows feedback message to user (can be implemented with UI)
    /// </summary>
    private void ShowFeedback(string message, Color color)
    {
        // You can implement this with a toast/notification system
        // For now, just log to console
        Debug.Log($"[Feedback] {message}");
    }

    /// <summary>
    /// Public method to manually trigger scanning
    /// </summary>
    public void StartScanning()
    {
        OnScanButtonClicked();
    }

    /// <summary>
    /// Public method to stop scanning
    /// </summary>
    public void StopScanning()
    {
        if (barcodeScanner != null)
        {
            barcodeScanner.StopScanning();
        }
    }
}
