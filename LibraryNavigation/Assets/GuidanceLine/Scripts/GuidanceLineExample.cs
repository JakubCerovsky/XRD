using UnityEngine;
using GuidanceLine;

/// <summary>
/// Example script showing how to use GuidanceLineAuto at runtime
/// This demonstrates setting destinations, checking progress, etc.
/// </summary>
public class GuidanceLineExample : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The GuidanceLineAuto component")]
    public GuidanceLineAuto guidanceLine;

    [Tooltip("Array of possible destinations")]
    public Transform[] destinations;

    [Header("Settings")]
    [Tooltip("Auto-select random destination on start")]
    public bool autoSelectDestination = true;

    [Tooltip("Switch to next destination when reached")]
    public bool autoProgress = true;

    private int currentDestinationIndex = 0;

    void Start()
    {
        if (guidanceLine == null)
        {
            guidanceLine = FindFirstObjectByType<GuidanceLineAuto>();
        }

        if (autoSelectDestination && destinations.Length > 0)
        {
            SetRandomDestination();
        }
    }

    void Update()
    {
        // Example: Press 'N' to go to next destination
        if (Input.GetKeyDown(KeyCode.N))
        {
            SetNextDestination();
        }

        // Example: Press 'R' to go to random destination
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetRandomDestination();
        }

        // Check if reached destination and auto-progress
        if (autoProgress && guidanceLine != null && guidanceLine.HasReachedDestination())
        {
            Debug.Log("Reached destination! Moving to next...");
            SetNextDestination();
        }

        // Display distance (optional)
        if (guidanceLine != null)
        {
            float distance = guidanceLine.GetDistanceToDestination();
            // You could update UI here: distanceText.text = $"{distance:F1}m";
        }
    }

    public void SetDestination(int index)
    {
        if (destinations == null || destinations.Length == 0)
        {
            Debug.LogWarning("No destinations set!");
            return;
        }

        if (index < 0 || index >= destinations.Length)
        {
            Debug.LogWarning($"Invalid destination index: {index}");
            return;
        }

        currentDestinationIndex = index;
        
        if (guidanceLine != null && destinations[index] != null)
        {
            guidanceLine.SetDestination(destinations[index].position);
            Debug.Log($"Navigation set to: {destinations[index].name}");
        }
    }

    public void SetNextDestination()
    {
        if (destinations == null || destinations.Length == 0)
            return;

        currentDestinationIndex = (currentDestinationIndex + 1) % destinations.Length;
        SetDestination(currentDestinationIndex);
    }

    public void SetRandomDestination()
    {
        if (destinations == null || destinations.Length == 0)
            return;

        int randomIndex = Random.Range(0, destinations.Length);
        SetDestination(randomIndex);
    }

    public void SetDestinationByName(string destinationName)
    {
        for (int i = 0; i < destinations.Length; i++)
        {
            if (destinations[i].name == destinationName)
            {
                SetDestination(i);
                return;
            }
        }
        Debug.LogWarning($"Destination '{destinationName}' not found!");
    }

    // Example: Call this from UI buttons
    public void OnDestinationButtonClicked(int destinationIndex)
    {
        SetDestination(destinationIndex);
    }

    // Example: For AR - set destination by world position
    public void NavigateToPosition(Vector3 worldPosition)
    {
        if (guidanceLine != null)
        {
            guidanceLine.SetDestination(worldPosition);
            Debug.Log($"Navigating to position: {worldPosition}");
        }
    }

    void OnGUI()
    {
        // Simple on-screen controls for testing
        if (destinations == null || destinations.Length == 0)
            return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 400));
        GUILayout.Label("Guidance Line Controls", GUI.skin.box);
        
        GUILayout.Label($"Current Destination: {destinations[currentDestinationIndex].name}");
        
        if (guidanceLine != null)
        {
            float distance = guidanceLine.GetDistanceToDestination();
            GUILayout.Label($"Distance: {distance:F1}m");
            
            bool reached = guidanceLine.HasReachedDestination();
            GUILayout.Label($"Reached: {(reached ? "Yes" : "No")}");
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Next Destination (N)"))
        {
            SetNextDestination();
        }

        if (GUILayout.Button("Random Destination (R)"))
        {
            SetRandomDestination();
        }

        GUILayout.Space(10);
        GUILayout.Label("Select Destination:");

        for (int i = 0; i < destinations.Length; i++)
        {
            if (GUILayout.Button($"{i + 1}. {destinations[i].name}"))
            {
                SetDestination(i);
            }
        }

        GUILayout.EndArea();
    }
}
