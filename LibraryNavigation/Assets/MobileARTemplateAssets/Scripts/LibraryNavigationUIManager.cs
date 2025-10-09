using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using GuidanceLine;

/// <summary>
/// Manages the UI flow for the Library Navigation AR app.
/// Sequence: Greeting Prompt -> Scan Button -> Book Search (ISBN input) -> Start Navigation
/// </summary>
public class LibraryNavigationUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("The greeting prompt panel shown at startup")]
    [SerializeField]
    private GameObject greetingPrompt;

    [Tooltip("The scan button panel shown after greeting is dismissed")]
    [SerializeField]
    private GameObject scanButton;

    [Tooltip("The book search panel with ISBN input")]
    [SerializeField]
    private GameObject bookSearchPanel;

    [Header("Buttons")]
    [Tooltip("Continue button on the greeting prompt")]
    [SerializeField]
    private Button continueButton;

    [Tooltip("Search button that opens the book search panel")]
    [SerializeField]
    private Button searchButton;

    [Tooltip("Search button within the book search panel to start navigation")]
    [SerializeField]
    private Button bookSearchButton;

    [Header("Input")]
    [Tooltip("ISBN input field in the book search panel")]
    [SerializeField]
    private TMP_InputField isbnInputField;

    [Header("Navigation")]
    [Tooltip("Reference to the GuidanceLineAuto component (optional - will auto-find if not assigned)")]
    [SerializeField]
    private GuidanceLineAuto guidanceLineAuto;

    [Tooltip("The book/target GameObject to navigate to (optional - will auto-find based on ISBN)")]
    [SerializeField]
    private Transform bookTargetTransform;

    [Header("Book Lookup Settings")]
    [Tooltip("Parent GameObject containing all book destinations (optional - searches entire scene if not set)")]
    [SerializeField]
    private Transform booksParent;

    [Tooltip("Tag to use when searching for book GameObjects (leave empty to search by name only)")]
    [SerializeField]
    private string bookTag = "Book";

    [Tooltip("Tag to use when searching for the GuidanceLine GameObject")]
    [SerializeField]
    private string lineTag = "Line";

    [Header("Debug")]
    [Tooltip("Show debug messages in console")]
    [SerializeField]
    private bool showDebugLogs = true;

    private enum UIState
    {
        Greeting,
        ScanButton,
        BookSearch,
        Navigating
    }

    // Removed unused field warning - keeping for future use if needed
    // private UIState currentState = UIState.Greeting;

    // Cache the LineRenderer once found to avoid repeated searches
    private LineRenderer cachedLineRenderer;

    void Start()
    {
        // Wire up button events
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
        else
            LogWarning("Continue Button is not assigned!");

        if (searchButton != null)
            searchButton.onClick.AddListener(OnSearchButtonClicked);
        else
            LogWarning("Search Button is not assigned!");

        if (bookSearchButton != null)
            bookSearchButton.onClick.AddListener(OnBookSearchClicked);
        else
            LogWarning("Book Search Button is not assigned!");

        // Hide the guidance line at startup
        HideGuidanceLine();

        // Initialize UI state
        ShowGreetingPrompt();
    }

    void OnDestroy()
    {
        // Clean up button listeners
        if (continueButton != null)
            continueButton.onClick.RemoveListener(OnContinueClicked);
        if (searchButton != null)
            searchButton.onClick.RemoveListener(OnSearchButtonClicked);
        if (bookSearchButton != null)
            bookSearchButton.onClick.RemoveListener(OnBookSearchClicked);
    }

    /// <summary>
    /// Shows the greeting prompt and hides all other UI elements.
    /// </summary>
    void ShowGreetingPrompt()
    {
        SetPanelActive(greetingPrompt, true);
        SetPanelActive(scanButton, false);
        SetPanelActive(bookSearchPanel, false);
        Log("Showing Greeting Prompt");
    }

    /// <summary>
    /// Called when the Continue button is clicked on the greeting prompt.
    /// </summary>
    void OnContinueClicked()
    {
        Log("Continue button clicked");
        ShowScanButton();
    }

    /// <summary>
    /// Shows the scan button and hides other UI elements.
    /// </summary>
    void ShowScanButton()
    {
        SetPanelActive(greetingPrompt, false);
        SetPanelActive(scanButton, true);
        SetPanelActive(bookSearchPanel, false);
        Log("Showing Scan Button");
    }

    /// <summary>
    /// Called when the Search button is clicked.
    /// </summary>
    void OnSearchButtonClicked()
    {
        Log("Search button clicked");
        ShowBookSearchPanel();
    }

    /// <summary>
    /// Shows the book search panel with ISBN input.
    /// </summary>
    void ShowBookSearchPanel()
    {
        SetPanelActive(greetingPrompt, false);
        SetPanelActive(scanButton, false);
        SetPanelActive(bookSearchPanel, true);
        
        // Clear previous ISBN input
        if (isbnInputField != null)
            isbnInputField.text = "";
        
        Log("Showing Book Search Panel");
    }

    /// <summary>
    /// Called when the user clicks Search in the book search panel.
    /// Starts the navigation to the book.
    /// </summary>
    void OnBookSearchClicked()
    {
        string isbn = isbnInputField != null ? isbnInputField.text : "";
        Log($"Book Search button clicked. ISBN: {isbn}");

        // Validate ISBN input
        if (string.IsNullOrEmpty(isbn))
        {
            LogWarning("ISBN input is empty!");
            return;
        }

        // Start navigation
        StartNavigation(isbn);
    }

    /// <summary>
    /// Starts the navigation from the camera to the book target.
    /// </summary>
    /// <param name="isbn">The ISBN entered by the user</param>
    void StartNavigation(string isbn)
    {
        // Hide all UI panels
        SetPanelActive(greetingPrompt, false);
        SetPanelActive(scanButton, false);
        SetPanelActive(bookSearchPanel, false);

        // Auto-find GuidanceLineAuto if not assigned
        if (guidanceLineAuto == null)
        {
            Log("GuidanceLineAuto not assigned, searching in scene...");
            guidanceLineAuto = FindFirstObjectByType<GuidanceLineAuto>();
            
            if (guidanceLineAuto != null)
            {
                Log($"Found GuidanceLineAuto on GameObject: {guidanceLineAuto.gameObject.name}");
            }
            else
            {
                LogError("GuidanceLineAuto component not found in scene! Please add it to a GameObject.");
                return;
            }
        }

        // Auto-find book target if not assigned
        if (bookTargetTransform == null)
        {
            Log($"Book Target not assigned, searching for book with ISBN: {isbn}...");
            bookTargetTransform = FindBookByISBN(isbn);
        }

        if (bookTargetTransform == null)
        {
            LogError($"Book with ISBN '{isbn}' not found! Make sure a GameObject named '{isbn}' or tagged '{bookTag}' exists in the scene.");
            return;
        }

        // Enable the guidance line component if it's disabled
        if (!guidanceLineAuto.enabled)
            guidanceLineAuto.enabled = true;

        // Set the destination - this will automatically calculate the path
        guidanceLineAuto.SetDestination(bookTargetTransform.position);
        
        // Now show the guidance line
        ShowGuidanceLine();
        
        Log($"✓ Book location found! Navigation started to '{bookTargetTransform.name}'");
        Log($"Path: Main Camera → {bookTargetTransform.name} (ISBN: {isbn})");
        Log("GuidanceLine is now visible showing the path to the book!");
    }

    /// <summary>
    /// Finds a book GameObject by ISBN. Searches by name or tag across all loaded scenes.
    /// </summary>
    /// <param name="isbn">The ISBN to search for</param>
    /// <returns>The Transform of the book GameObject, or null if not found</returns>
    Transform FindBookByISBN(string isbn)
    {
        Transform foundBook = null;

        // Strategy 1: Search within booksParent if assigned
        if (booksParent != null)
        {
            Log($"Searching for book in '{booksParent.name}' GameObject...");
            
            // Try finding by name first (exact match or contains ISBN)
            foreach (Transform child in booksParent)
            {
                if (child.name == isbn || child.name.Contains(isbn))
                {
                    foundBook = child;
                    Log($"Found book by name: {child.name}");
                    return foundBook;
                }
            }

            // Try finding by tag if specified
            if (!string.IsNullOrEmpty(bookTag))
            {
                foreach (Transform child in booksParent)
                {
                    if (child.CompareTag(bookTag))
                    {
                        foundBook = child;
                        Log($"Found book by tag '{bookTag}': {child.name}");
                        return foundBook;
                    }
                }
            }
        }

        // Strategy 2: Search by tag across all loaded scenes
        if (!string.IsNullOrEmpty(bookTag))
        {
            Log($"Searching all loaded scenes for GameObjects with tag '{bookTag}'...");
            
            try
            {
                GameObject[] taggedBooks = GameObject.FindGameObjectsWithTag(bookTag);
                Log($"Found {taggedBooks.Length} GameObject(s) with tag '{bookTag}'");
                
                if (taggedBooks.Length > 0)
                {
                    // If multiple books with the tag, try to find one that matches ISBN in name
                    foreach (var book in taggedBooks)
                    {
                        Log($"  - Found tagged book: '{book.name}' in scene '{book.scene.name}'");
                        if (book.name == isbn || book.name.Contains(isbn))
                        {
                            foundBook = book.transform;
                            Log($"✓ Found book by tag and name match: {book.name} in scene '{book.scene.name}'");
                            return foundBook;
                        }
                    }

                    // Otherwise return the first tagged book
                    foundBook = taggedBooks[0].transform;
                    Log($"✓ Using first book found with tag '{bookTag}': {foundBook.name} in scene '{foundBook.gameObject.scene.name}'");
                    return foundBook;
                }
                else
                {
                    LogWarning($"No GameObjects found with tag '{bookTag}'. Make sure the tag exists and is assigned.");
                }
            }
            catch (UnityException e)
            {
                LogError($"Tag '{bookTag}' is not defined! Error: {e.Message}");
                LogError("Go to Edit → Project Settings → Tags and Layers to add the 'Book' tag.");
            }
        }

        // Strategy 3: Search all loaded scenes by name
        Log($"Searching all loaded scenes for GameObject named '{isbn}' or 'Book'...");
        Log($"Total loaded scenes: {SceneManager.sceneCount}");
        
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
            {
                Log($"  - Scene {i}: '{scene.name}' (not loaded, skipping)");
                continue;
            }

            Log($"  - Searching in scene {i}: '{scene.name}' (loaded)");
            
            // Get all root GameObjects in the scene
            GameObject[] rootObjects = scene.GetRootGameObjects();
            Log($"    Found {rootObjects.Length} root GameObject(s) in this scene");
            
            foreach (GameObject rootObj in rootObjects)
            {
                Log($"      Searching hierarchy of: '{rootObj.name}'");
                // Search this object and all its children
                foundBook = SearchInHierarchy(rootObj.transform, isbn);
                if (foundBook != null)
                {
                    Log($"✓ Found book '{foundBook.name}' in scene '{scene.name}'");
                    return foundBook;
                }
            }
        }

        // Strategy 4: Look for common parent names across all scenes
        string[] commonParentNames = { "Destinations", "Books", "Book", "Targets", "LibraryEnvironment" };
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded) continue;

            foreach (string parentName in commonParentNames)
            {
                GameObject[] rootObjects = scene.GetRootGameObjects();
                foreach (GameObject rootObj in rootObjects)
                {
                    Transform parent = FindChildRecursive(rootObj.transform, parentName);
                    if (parent != null)
                    {
                        Log($"Searching in '{parentName}' GameObject in scene '{scene.name}'...");
                        foundBook = SearchInHierarchy(parent, isbn);
                        if (foundBook != null)
                        {
                            Log($"Found book '{foundBook.name}' in '{parentName}'");
                            return foundBook;
                        }
                    }
                }
            }
        }

        LogWarning($"Book with ISBN '{isbn}' not found using any search strategy across {SceneManager.sceneCount} loaded scene(s).");
        return null;
    }

    /// <summary>
    /// Recursively searches for a GameObject by name in a hierarchy.
    /// </summary>
    Transform SearchInHierarchy(Transform parent, string searchName)
    {
        // Check if parent matches (exact match or contains search name, or is named "Book")
        if (parent.name == searchName || parent.name.Contains(searchName) || parent.name == "Book")
        {
            Log($"        ✓ Match found: '{parent.name}'");
            return parent;
        }

        // Search children recursively
        foreach (Transform child in parent)
        {
            Transform result = SearchInHierarchy(child, searchName);
            if (result != null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Recursively finds a child Transform by name.
    /// </summary>
    Transform FindChildRecursive(Transform parent, string childName)
    {
        if (parent.name == childName)
            return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindChildRecursive(child, childName);
            if (result != null)
                return result;
        }

        return null;
    }

    /// <summary>
    /// Hides the guidance line by finding LineRenderer component across all loaded scenes.
    /// </summary>
    void HideGuidanceLine()
    {
        LineRenderer lineRenderer = FindGuidanceLineRenderer();
        
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
            Log($"✓ Hidden GuidanceLine on GameObject: '{lineRenderer.gameObject.name}' in scene '{lineRenderer.gameObject.scene.name}'");
        }
        else
        {
            LogWarning($"LineRenderer component not found across {SceneManager.sceneCount} loaded scene(s). The line may not exist yet.");
        }
    }

    /// <summary>
    /// Shows the guidance line by finding LineRenderer component across all loaded scenes.
    /// </summary>
    void ShowGuidanceLine()
    {
        LineRenderer lineRenderer = FindGuidanceLineRenderer();
        
        if (lineRenderer != null)
        {
            lineRenderer.enabled = true;
            Log($"✓ Showing GuidanceLine on GameObject: '{lineRenderer.gameObject.name}' in scene '{lineRenderer.gameObject.scene.name}'");
        }
        else
        {
            LogWarning($"LineRenderer component not found. The line may not be visible.");
        }
    }

    /// <summary>
    /// Finds the LineRenderer component on the CurvedLine GameObject across all loaded scenes.
    /// Specifically looks for CurvedLine to avoid finding AR plane LineRenderers.
    /// Caches the result to avoid repeated searches.
    /// </summary>
    LineRenderer FindGuidanceLineRenderer()
    {
        // Return cached LineRenderer if already found
        if (cachedLineRenderer != null)
        {
            return cachedLineRenderer;
        }

        Log($"Searching for CurvedLine LineRenderer component across {SceneManager.sceneCount} loaded scene(s)...");
        
        // Strategy 1: Look for GameObject named "CurvedLine" specifically
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (!scene.isLoaded)
            {
                Log($"  - Scene {i}: '{scene.name}' (not loaded, skipping)");
                continue;
            }

            Log($"  - Searching in scene {i}: '{scene.name}' (loaded)");
            
            // Get all root GameObjects in the scene
            GameObject[] rootObjects = scene.GetRootGameObjects();
            
            foreach (GameObject rootObj in rootObjects)
            {
                // Search for "CurvedLine" GameObject specifically
                Transform curvedLineTransform = FindChildRecursive(rootObj.transform, "CurvedLine");
                if (curvedLineTransform != null)
                {
                    LineRenderer lineRenderer = curvedLineTransform.GetComponent<LineRenderer>();
                    if (lineRenderer != null)
                    {
                        Log($"✓ Found LineRenderer on CurvedLine in scene '{scene.name}'");
                        cachedLineRenderer = lineRenderer; // Cache it
                        return lineRenderer;
                    }
                }
            }
        }

        // Strategy 2: Look for any GameObject with "Line" tag
        if (!string.IsNullOrEmpty(lineTag))
        {
            try
            {
                GameObject[] lineObjects = GameObject.FindGameObjectsWithTag(lineTag);
                if (lineObjects.Length > 0)
                {
                    foreach (var lineObj in lineObjects)
                    {
                        // Skip AR planes - only look for objects with "Line" or "Curved" in the name
                        if (!lineObj.name.ToLower().Contains("line") && !lineObj.name.ToLower().Contains("curved"))
                            continue;

                        LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
                        if (lineRenderer != null)
                        {
                            Log($"✓ Found LineRenderer on tagged GameObject '{lineObj.name}' in scene '{lineObj.scene.name}'");
                            cachedLineRenderer = lineRenderer; // Cache it
                            return lineRenderer;
                        }
                    }
                }
            }
            catch (UnityException e)
            {
                LogWarning($"Tag '{lineTag}' issue: {e.Message}");
            }
        }

        return null;
    }

    /// <summary>
    /// Resets the UI to the greeting prompt (useful for testing or restarting).
    /// </summary>
    public void ResetUI()
    {
        ShowGreetingPrompt();
        HideGuidanceLine();
        if (guidanceLineAuto != null)
            guidanceLineAuto.enabled = false;
        Log("UI Reset to Greeting Prompt");
    }

    /// <summary>
    /// Helper to safely activate/deactivate panels with null checks.
    /// </summary>
    void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }

    void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log($"[LibraryNavigationUI] {message}");
    }

    void LogWarning(string message)
    {
        Debug.LogWarning($"[LibraryNavigationUI] {message}");
    }

    void LogError(string message)
    {
        Debug.LogError($"[LibraryNavigationUI] {message}");
    }
}
