using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GuidanceLine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

/// <summary>
/// Simple UI controller for the Library Navigation flow.
/// Attach this to your UI root GameObject and assign the panels/buttons in the inspector (or let it auto-find them).
/// Flow:
///  - Greeting Prompt visible at start
///  - Continue -> shows Start Button
///  - Start Button -> shows Search Book Prompt
///  - Search Book Button -> checks BookDatabase for ISBN; if found starts navigation
///  - Leave Button -> stops navigation and returns to Start Button
///  - When destination reached -> stops navigation and returns to Start Button
/// </summary>
public class SimpleLibraryUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject greetingPrompt;
    [SerializeField] private GameObject startButtonPanel;
    [SerializeField] private GameObject searchBookPrompt;
    [SerializeField] private GameObject leaveButtonPanel;

    [Header("Buttons & Inputs")]
    [SerializeField] private Button greetingContinueButton;
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField isbnInputField;
    [SerializeField] private Button searchBookButton;
    [SerializeField] private Button leaveButton;

    [Header("Dependencies")]
    [SerializeField] private GuidanceLineAuto guidanceLineAuto;
    [SerializeField] private BookDatabase bookDatabase;
    [SerializeField] private LineRenderer curvedLineRenderer; // serialized so runtime assignment is visible in Inspector
    [SerializeField] private ARTrackedImageManager trackedImageManager;

    [Header("Settings")]
    [Tooltip("Distance in meters to consider destination reached")]
    [SerializeField] private float destinationReachedDistance = 1.5f;

    [Header("Runtime Detection")]
    [Tooltip("Enable continuous detection of BookDatabase and CurvedLine (useful when they load after image scan)")]
    [SerializeField] private bool enableRuntimeDetection = true;

    [Tooltip("Check for missing dependencies every N seconds")]
    [SerializeField] private float runtimeDetectionInterval = 0.5f;

    private Transform currentBookTransform;
    private bool isNavigating = false;
    private float lastDestinationCheck = 0f;
    private float destinationCheckInterval = 0.5f;
    private float lastRuntimeDetectionCheck = 0f;

    void Start()
    {
        // Wire up events safely
        if (greetingContinueButton != null) greetingContinueButton.onClick.AddListener(OnGreetingContinue);
        if (startButton != null) startButton.onClick.AddListener(OnStartClicked);
        if (searchBookButton != null) searchBookButton.onClick.AddListener(OnSearchBookClicked);
        if (leaveButton != null) leaveButton.onClick.AddListener(OnLeaveClicked);

        // Auto-find dependencies if not assigned
        if (guidanceLineAuto == null)
            guidanceLineAuto = FindFirstObjectByType<GuidanceLineAuto>();
        if (bookDatabase == null)
            bookDatabase = FindFirstObjectByType<BookDatabase>();
        if (trackedImageManager == null)
            trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();

        // Subscribe to tracked image events so we can run detection immediately when an image is detected
        if (trackedImageManager != null)
        {
            trackedImageManager.trackablesChanged.AddListener(OnTrackedImagesChanged);
        }

        // Also subscribe to sceneLoaded so we can try detection when new scenes are loaded
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initial UI state
        SetPanelActive(greetingPrompt, true);
        SetPanelActive(startButtonPanel, false);
        SetPanelActive(searchBookPrompt, false);
        SetPanelActive(leaveButtonPanel, false);

        // Hide all book objects at startup so they only appear when targeted
        HideAllBooksAtStart();
    }

    void Update()
    {
        // Continuous runtime detection for dynamically loaded objects
        if (enableRuntimeDetection && Time.time - lastRuntimeDetectionCheck >= runtimeDetectionInterval)
        {
            lastRuntimeDetectionCheck = Time.time;
            TryFindMissingDependencies();
        }

        if (isNavigating && currentBookTransform != null && Time.time - lastDestinationCheck >= destinationCheckInterval)
        {
            lastDestinationCheck = Time.time;
            CheckDestinationReached();
        }

        // Also check the GuidanceLineAuto state in case it determined arrival itself
        if (isNavigating && guidanceLineAuto != null)
        {
            try
            {
                if (guidanceLineAuto.HasReachedDestination())
                {
                    Debug.Log("[SimpleLibraryUI] GuidanceLineAuto reports destination reached - stopping navigation.");
                    StopNavigation();
                }
            }
            catch { }
        }
    }

    void OnDestroy()
    {
        if (greetingContinueButton != null) greetingContinueButton.onClick.RemoveListener(OnGreetingContinue);
        if (startButton != null) startButton.onClick.RemoveListener(OnStartClicked);
        if (searchBookButton != null) searchBookButton.onClick.RemoveListener(OnSearchBookClicked);
        if (leaveButton != null) leaveButton.onClick.RemoveListener(OnLeaveClicked);

        if (trackedImageManager != null)
            trackedImageManager.trackablesChanged.RemoveListener(OnTrackedImagesChanged);

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When a scene is loaded (for example the simulated environment), try to find dependencies immediately
        TryFindMissingDependencies();
    }

    void OnTrackedImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        // If any images were added or updated, run detection immediately
        if ((args.added != null && args.added.Count > 0) || (args.updated != null && args.updated.Count > 0))
        {
            TryFindMissingDependencies();
        }
    }

    /// <summary>
    /// Continuously tries to find BookDatabase and CurvedLine if they are not yet assigned.
    /// Called periodically to handle dynamically loaded objects (e.g., after image scan).
    /// </summary>
    [ContextMenu("Try Find Missing Dependencies")]
    void TryFindMissingDependencies()
    {
        // Try to find BookDatabase if not assigned
        if (bookDatabase == null)
        {
            bookDatabase = FindFirstObjectByType<BookDatabase>();
            if (bookDatabase != null)
            {
                Debug.Log("[SimpleLibraryUI] Found BookDatabase at runtime!");
                // Ensure any books registered in the database are hidden initially
                HideBooksFromDatabase(bookDatabase);
            }
        }
        // Try to find CurvedLine LineRenderer if not cached
        if (curvedLineRenderer == null)
        {
            // Strategy 1: Find by name "CurvedLine"
            GameObject curvedLineObj = GameObject.Find("CurvedLine");
            if (curvedLineObj != null)
            {
                curvedLineRenderer = curvedLineObj.GetComponent<LineRenderer>();
                if (curvedLineRenderer != null)
                {
                    Debug.Log("[SimpleLibraryUI] Found CurvedLine LineRenderer at runtime!");
                    // If we detected a GuidanceLineAuto, tell it to use this external renderer
                    if (guidanceLineAuto != null)
                    {
                        guidanceLineAuto.SetExternalLineRenderer(curvedLineRenderer);
                        Debug.Log("[SimpleLibraryUI] Assigned external LineRenderer to GuidanceLineAuto.");
                    }
                }
            }

            // Strategy 2: Look for any LineRenderer with "Line" tag
            if (curvedLineRenderer == null)
            {
                try
                {
                    GameObject[] lineObjects = GameObject.FindGameObjectsWithTag("Line");
                    foreach (GameObject obj in lineObjects)
                    {
                        LineRenderer lr = obj.GetComponent<LineRenderer>();
                        if (lr != null && (obj.name.Contains("Curved") || obj.name.Contains("Line")))
                        {
                            curvedLineRenderer = lr;
                            Debug.Log($"[SimpleLibraryUI] Found CurvedLine LineRenderer by tag: {obj.name}");
                            break;
                        }
                    }
                }
                catch { }
            }

            // If we found a curved line renderer, also attempt to find GuidanceLineAuto (it may live in the same dynamic scene)
            if (curvedLineRenderer != null && guidanceLineAuto == null)
            {
                // 1) try to find a GuidanceLineAuto component on the same GameObject or a parent
                var candidate = curvedLineRenderer.gameObject.GetComponentInParent<GuidanceLineAuto>();
                if (candidate != null)
                {
                    guidanceLineAuto = candidate;
                    Debug.Log("[SimpleLibraryUI] Assigned GuidanceLineAuto from CurvedLine parent at runtime.");
                    // If we already have a curvedLineRenderer, ensure the guidance uses it
                    if (curvedLineRenderer != null)
                    {
                        guidanceLineAuto.SetExternalLineRenderer(curvedLineRenderer);
                        Debug.Log("[SimpleLibraryUI] Assigned external LineRenderer to GuidanceLineAuto from parent candidate.");
                    }
                }

                // 2) fallback: global search for GuidanceLineAuto (if it's created elsewhere)
                if (guidanceLineAuto == null)
                {
                    var foundGuidance = FindFirstObjectByType<GuidanceLineAuto>();
                    if (foundGuidance != null)
                    {
                        guidanceLineAuto = foundGuidance;
                        Debug.Log("[SimpleLibraryUI] Found GuidanceLineAuto at runtime (global search)!");
                        if (curvedLineRenderer != null)
                        {
                            guidanceLineAuto.SetExternalLineRenderer(curvedLineRenderer);
                            Debug.Log("[SimpleLibraryUI] Assigned external LineRenderer to GuidanceLineAuto from global search.");
                        }
                    }
                }

                // 3) deeper search: traverse all loaded scenes and root objects and search children (including inactive)
                if (guidanceLineAuto == null)
                {
                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        var scene = SceneManager.GetSceneAt(i);
                        if (!scene.isLoaded) continue;
                        var roots = scene.GetRootGameObjects();
                        foreach (var root in roots)
                        {
                            var ga = root.GetComponentInChildren<GuidanceLineAuto>(true);
                            if (ga != null)
                            {
                                guidanceLineAuto = ga;
                                Debug.Log($"[SimpleLibraryUI] Found GuidanceLineAuto in scene '{scene.name}' via deep search.");
                                if (curvedLineRenderer != null)
                                {
                                    guidanceLineAuto.SetExternalLineRenderer(curvedLineRenderer);
                                    Debug.Log("[SimpleLibraryUI] Assigned external LineRenderer to GuidanceLineAuto from deep search.");
                                }
                                break;
                            }
                        }
                        if (guidanceLineAuto != null) break;
                    }
                }
            }

            // If GuidanceLineAuto still not found, try a dedicated deep search for the CurvedLine's GameObject (in case GuidanceLineAuto is attached there)
            if (guidanceLineAuto == null)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (!scene.isLoaded) continue;
                    var roots = scene.GetRootGameObjects();
                    foreach (var root in roots)
                    {
                        var curved = root.transform.Find("CurvedLine");
                        if (curved != null)
                        {
                            var ga = curved.GetComponentInChildren<GuidanceLineAuto>(true);
                            if (ga != null)
                            {
                                guidanceLineAuto = ga;
                                Debug.Log($"[SimpleLibraryUI] Found GuidanceLineAuto under CurvedLine in scene '{scene.name}'.");
                                break;
                            }
                        }
                    }
                    if (guidanceLineAuto != null) break;
                }
            }
        }
    }

    /// <summary>
    /// Hides all GameObjects tagged with 'Book' at startup.
    /// </summary>
    void HideAllBooksAtStart()
    {
        try
        {
            GameObject[] found = GameObject.FindGameObjectsWithTag("Book");
            foreach (var go in found)
            {
                if (go != null)
                    go.SetActive(false);
            }

            Debug.Log($"[SimpleLibraryUI] Hid {found.Length} book GameObject(s) at startup (tag 'Book').");
        }
        catch (UnityException)
        {
            Debug.LogWarning("[SimpleLibraryUI] Tag 'Book' not defined. Skipped hiding by tag.");
        }
    }

    /// <summary>
    /// Hides all books registered in the BookDatabase (if any).
    /// </summary>
    void HideBooksFromDatabase(BookDatabase db)
    {
        if (db == null) return;
        try
        {
            var entries = db.GetAllBooks();
            int count = 0;
            foreach (var e in entries)
            {
                if (e != null && e.bookObject != null)
                {
                    e.bookObject.SetActive(false);
                    count++;
                }
            }
            Debug.Log($"[SimpleLibraryUI] Hid {count} book(s) from BookDatabase.");
        }
        catch (System.Exception)
        {
            // Defensive: if BookEntry type isn't accessible or something unexpected, ignore
        }
    }

    /// <summary>
    /// Shows only the provided book GameObject and hides all others (by tag and DB).
    /// </summary>
    void ShowOnlyBook(GameObject bookObj)
    {
        // Hide all known books first
        HideAllBooksAtStart();
        if (bookDatabase != null)
            HideBooksFromDatabase(bookDatabase);

        if (bookObj != null)
        {
            bookObj.SetActive(true);
            Debug.Log($"[SimpleLibraryUI] Showing target book: {bookObj.name}");
        }
    }

    void OnGreetingContinue()
    {
        SetPanelActive(greetingPrompt, false);
        SetPanelActive(startButtonPanel, true);
    }

    void OnStartClicked()
    {
        SetPanelActive(startButtonPanel, false);
        SetPanelActive(searchBookPrompt, true);

        // clear previous input
        if (isbnInputField != null) isbnInputField.text = string.Empty;
    }

    void OnSearchBookClicked()
    {
        string isbn = isbnInputField != null ? isbnInputField.text.Trim() : string.Empty;
        if (string.IsNullOrEmpty(isbn))
        {
            Debug.LogWarning("ISBN is empty");
            return;
        }

        GameObject bookObj = null;
        if (bookDatabase != null)
            bookObj = bookDatabase.FindBookByISBN(isbn);

        // Fallback: try to find object by name/tag in scene
        if (bookObj == null)
        {
            // Try tag
            if (!string.IsNullOrEmpty(bookDatabase != null ? "Book" : ""))
            {
                try
                {
                    var objs = GameObject.FindGameObjectsWithTag("Book");
                    foreach (var o in objs)
                    {
                        if (o.name == isbn || o.name.Contains(isbn))
                        {
                            bookObj = o;
                            break;
                        }
                    }
                }
                catch { }
            }

            if (bookObj == null)
            {
                // find by name across scenes
                var g = GameObject.Find(isbn);
                if (g != null) bookObj = g;
            }
        }

        if (bookObj == null)
        {
            Debug.LogWarning($"Book with ISBN '{isbn}' not found");
            return;
        }

        // Start navigation
        currentBookTransform = bookObj.transform;

        // Set guidance line start and end using GuidanceLineAuto public API so it calculates path immediately
        if (guidanceLineAuto != null)
        {
            // If we have a CurvedLine LineRenderer already, tell GuidanceLineAuto to use it
            if (curvedLineRenderer != null)
            {
                guidanceLineAuto.SetExternalLineRenderer(curvedLineRenderer);
                Debug.Log("[SimpleLibraryUI] Set GuidanceLineAuto external LineRenderer before calculating path.");
            }

            // start point = main camera (use the public setter which triggers recalculation)
            if (Camera.main != null)
                guidanceLineAuto.SetStartPoint(Camera.main.transform);

            // set the destination (this calls CalculatePath internally)
            guidanceLineAuto.SetDestination(currentBookTransform.position);

            // enable the component so Update can DrawCurvedLine and enable the renderer
            guidanceLineAuto.enabled = true;
        }

    // Make only the target book visible
    ShowOnlyBook(bookObj);

    SetPanelActive(searchBookPrompt, false);
        SetPanelActive(leaveButtonPanel, true);

        isNavigating = true;
        lastDestinationCheck = Time.time;
    }

    void OnLeaveClicked()
    {
        StopNavigation();
    }

    void StopNavigation()
    {
        isNavigating = false;

        // hide guidance
        if (guidanceLineAuto != null)
        {
            guidanceLineAuto.enabled = false;
        }

        // hide the CurvedLine renderer
        if (curvedLineRenderer == null)
            TryFindMissingDependencies(); // One more attempt to find it

        if (curvedLineRenderer != null)
        {
            curvedLineRenderer.enabled = false;
        }

        // clear target
        // hide the target book again
        if (currentBookTransform != null)
        {
            currentBookTransform.gameObject.SetActive(false);
        }
        currentBookTransform = null;

        SetPanelActive(leaveButtonPanel, false);
        SetPanelActive(startButtonPanel, true);
    }

    void CheckDestinationReached()
    {
        if (currentBookTransform == null || Camera.main == null) return;

        float distance = Vector3.Distance(Camera.main.transform.position, currentBookTransform.position);
        if (distance <= destinationReachedDistance)
        {
            // reached
            StopNavigation();
        }
    }

    void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}
