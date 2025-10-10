using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Validates ISBN input in real-time and changes the input field color based on validity.
/// Attach this to a GameObject with a TMP_InputField component.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class ISBNInputValidator : MonoBehaviour
{
    [Header("Visual Feedback Colors")]
    [Tooltip("Color when ISBN is valid and found in database")]
    [SerializeField]
    private Color validColor = Color.white;

    [Tooltip("Color when ISBN format is invalid")]
    [SerializeField]
    private Color invalidColor = new Color(1f, 0.6f, 0.6f); // Light red

    [Tooltip("Color when ISBN is valid but not found in database")]
    [SerializeField]
    private Color notFoundColor = new Color(1f, 1f, 0.6f); // Light yellow

    [Header("Validation Settings")]
    [Tooltip("Show validation while typing (true) or only on submit (false)")]
    [SerializeField]
    private bool validateOnChange = true;

    [Tooltip("Allow empty input to be considered valid")]
    [SerializeField]
    private bool allowEmpty = true;

    [Tooltip("Check if book exists in database (not just format validation)")]
    [SerializeField]
    private bool checkBookDatabase = true;

    [Header("References")]
    [Tooltip("Reference to the BookDatabase component")]
    [SerializeField]
    private BookDatabase bookDatabase;

    private TMP_InputField inputField;
    private Image backgroundImage;
    private bool isValid = true;

    public enum ValidationState
    {
        Valid,          // ISBN is valid and found in database
        InvalidFormat,  // ISBN format is invalid
        NotFoundInDB    // ISBN format is valid but not in database
    }

    private ValidationState currentState = ValidationState.Valid;

    public bool IsValid => isValid;
    public ValidationState CurrentState => currentState;

    void Awake()
    {
        inputField = GetComponent<TMP_InputField>();

        // Get the background image for coloring
        backgroundImage = GetComponent<Image>();

        if (backgroundImage == null)
        {
            Debug.LogWarning("[ISBNInputValidator] No Image component found. Visual feedback will not work.");
        }
    }

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("[ISBNInputValidator] TMP_InputField component not found!");
            return;
        }

        // Try to find BookDatabase if not assigned
        FindBookDatabaseIfNeeded();

        // Subscribe to input field events
        if (validateOnChange)
        {
            inputField.onValueChanged.AddListener(OnInputChanged);
        }

        inputField.onEndEdit.AddListener(OnInputEndEdit);

        // Set initial color
        UpdateVisualFeedback(ValidationState.Valid);
    }

    /// <summary>
    /// Finds the BookDatabase in the scene if not already assigned.
    /// Called at Start and before each validation if database is still null.
    /// </summary>
    private void FindBookDatabaseIfNeeded()
    {
        if (!checkBookDatabase)
        {
            return;
        }

        if (bookDatabase == null)
        {
            // Try to find BookDatabase including inactive objects
            bookDatabase = FindObjectOfType<BookDatabase>(true);
            
            if (bookDatabase == null)
            {
                Debug.LogError("[ISBNInputValidator] ❌ BookDatabase not found in scene!");
                Debug.LogError("[ISBNInputValidator] SOLUTION: Drag 'Assets/LibraryEnvironment.prefab' into your scene hierarchy.");
                Debug.LogError("[ISBNInputValidator] The BookDatabase component is inside this prefab.");
            }
            else
            {
                Debug.Log($"[ISBNInputValidator] ✓ BookDatabase found automatically on GameObject: '{bookDatabase.gameObject.name}'");
                Debug.Log($"[ISBNInputValidator] Database contains {bookDatabase.GetBookCount()} book(s)");
            }
        }
    }

    void OnDestroy()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.RemoveListener(OnInputChanged);
            inputField.onEndEdit.RemoveListener(OnInputEndEdit);
        }
    }

    /// <summary>
    /// Called when the input value changes (while typing)
    /// </summary>
    private void OnInputChanged(string value)
    {
        ValidateInput(value);
    }

    /// <summary>
    /// Called when editing ends (user presses enter or clicks away)
    /// </summary>
    private void OnInputEndEdit(string value)
    {
        ValidateInput(value);
    }

    /// <summary>
    /// Validates the ISBN input and updates visual feedback
    /// </summary>
    private void ValidateInput(string value)
    {
        // Check if empty input is allowed
        if (string.IsNullOrWhiteSpace(value))
        {
            isValid = allowEmpty;
            currentState = allowEmpty ? ValidationState.Valid : ValidationState.InvalidFormat;
            UpdateVisualFeedback(currentState);
            return;
        }

        // First, check if the format is valid
        bool formatValid = ISBNFormatChecker.IsValidIsbn(value);

        if (!formatValid)
        {
            isValid = false;
            currentState = ValidationState.InvalidFormat;
            UpdateVisualFeedback(currentState);
            Debug.Log($"[ISBNInputValidator] Invalid ISBN format: '{value}'");
            return;
        }

        // If format is valid and database check is enabled, check if book exists in database
        if (checkBookDatabase)
        {
            // Try to find database if it wasn't found before (in case prefab was instantiated late)
            FindBookDatabaseIfNeeded();
            
            if (bookDatabase != null)
            {
                GameObject bookObject = bookDatabase.FindBookByISBN(value);
                
                if (bookObject != null)
                {
                    isValid = true;
                    currentState = ValidationState.Valid;
                    Debug.Log($"[ISBNInputValidator] ✓ Valid ISBN and book found in database: '{value}' -> {bookObject.name}");
                }
                else
                {
                    isValid = false;
                    currentState = ValidationState.NotFoundInDB;
                    Debug.Log($"[ISBNInputValidator] ⚠ Valid ISBN format but book not found in database: '{value}'");
                }

                UpdateVisualFeedback(currentState);
            }
            else
            {
                // Database still not found - show as format valid only
                isValid = true;
                currentState = ValidationState.Valid;
                UpdateVisualFeedback(currentState);
                Debug.LogWarning($"[ISBNInputValidator] BookDatabase not available. Only format validation performed for: '{value}'");
            }
        }
        else
        {
            // Only format validation - consider it valid if format is correct
            isValid = formatValid;
            currentState = ValidationState.Valid;
            UpdateVisualFeedback(currentState);
            Debug.Log($"[ISBNInputValidator] Valid ISBN format: '{value}' (database check disabled)");
        }
    }

    /// <summary>
    /// Updates the visual feedback based on validation state
    /// </summary>
    private void UpdateVisualFeedback(ValidationState state)
    {
        if (backgroundImage != null)
        {
            switch (state)
            {
                case ValidationState.Valid:
                    backgroundImage.color = validColor;
                    break;
                case ValidationState.InvalidFormat:
                    backgroundImage.color = invalidColor;
                    break;
                case ValidationState.NotFoundInDB:
                    backgroundImage.color = notFoundColor;
                    break;
            }
        }
    }

    /// <summary>
    /// Public method to manually validate the current input
    /// </summary>
    public bool ValidateCurrent()
    {
        ValidateInput(inputField.text);
        return isValid;
    }

    /// <summary>
    /// Public method to check if current input is valid without updating visuals
    /// </summary>
    public bool CheckValidity()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            return allowEmpty;
        }

        bool formatValid = ISBNFormatChecker.IsValidIsbn(inputField.text);

        if (!formatValid)
        {
            return false;
        }

        // If database check is enabled, also verify book exists
        if (checkBookDatabase && bookDatabase != null)
        {
            GameObject bookObject = bookDatabase.FindBookByISBN(inputField.text);
            return bookObject != null;
        }

        return formatValid;
    }

    /// <summary>
    /// Gets the book GameObject for the current ISBN input if it exists in the database
    /// </summary>
    public GameObject GetBookForCurrentISBN()
    {
        if (bookDatabase == null || string.IsNullOrWhiteSpace(inputField.text))
        {
            return null;
        }

        return bookDatabase.FindBookByISBN(inputField.text);
    }
}
