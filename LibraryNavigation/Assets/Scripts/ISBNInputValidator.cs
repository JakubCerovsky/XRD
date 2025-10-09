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
    [Tooltip("Color when ISBN is valid")]
    [SerializeField]
    private Color validColor = Color.white;

    [Tooltip("Color when ISBN is invalid")]
    [SerializeField]
    private Color invalidColor = new Color(1f, 0.6f, 0.6f); // Light red

    [Header("Validation Settings")]
    [Tooltip("Show validation while typing (true) or only on submit (false)")]
    [SerializeField]
    private bool validateOnChange = true;

    [Tooltip("Allow empty input to be considered valid")]
    [SerializeField]
    private bool allowEmpty = true;

    private TMP_InputField inputField;
    private Image backgroundImage;
    private bool isValid = true;

    public bool IsValid => isValid;

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

        // Subscribe to input field events
        if (validateOnChange)
        {
            inputField.onValueChanged.AddListener(OnInputChanged);
        }

        inputField.onEndEdit.AddListener(OnInputEndEdit);

        // Set initial color
        UpdateVisualFeedback(true);
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
            UpdateVisualFeedback(isValid);
            return;
        }

        // Use the ISBNFormatChecker to validate
        isValid = ISBNFormatChecker.IsValidIsbn(value);

        UpdateVisualFeedback(isValid);

        // Log for debugging
        if (!isValid)
        {
            Debug.Log($"[ISBNInputValidator] Invalid ISBN: '{value}'");
        }
        else
        {
            Debug.Log($"[ISBNInputValidator] Valid ISBN: '{value}'");
        }
    }

    /// <summary>
    /// Updates the visual feedback based on validation state
    /// </summary>
    private void UpdateVisualFeedback(bool valid)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = valid ? validColor : invalidColor;
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
        return ISBNFormatChecker.IsValidIsbn(inputField.text);
    }
}
