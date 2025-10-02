using UnityEngine;
using UnityEngine.EventSystems;

namespace GuidanceLine
{
    /// <summary>
    /// Ensures there's an EventSystem in the scene for Input System to work properly
    /// </summary>
    public class EventSystemChecker : MonoBehaviour
    {
        void Start()
        {
            // Check if EventSystem exists
            EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
            
            if (eventSystem == null)
            {
                // Create EventSystem
                GameObject eventSystemGO = new GameObject("EventSystem");
                eventSystemGO.AddComponent<EventSystem>();
                eventSystemGO.AddComponent<StandaloneInputModule>();
                
                Debug.Log("Created EventSystem for Input System compatibility");
            }
            else
            {
                Debug.Log("EventSystem already exists in scene");
            }
        }
    }
}