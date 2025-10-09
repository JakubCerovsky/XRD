using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageCubeSpawner : MonoBehaviour
{
    [SerializeField] ARTrackedImageManager trackedImageManager;
    [SerializeField] GameObject cubePrefab;

    // Keep one cube per reference image name
    Dictionary<string, GameObject> spawned = new Dictionary<string, GameObject>();

    void OnEnable()  => trackedImageManager.trackablesChanged.AddListener(OnChanged);
    void OnDisable() => trackedImageManager.trackablesChanged.RemoveListener(OnChanged);

    void OnChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var img in args.added)   UpdateFor(img);
        foreach (var img in args.updated) UpdateFor(img);

        foreach (var kvp in args.removed)
        {
            var img = kvp.Value;
            var key = img.referenceImage.name;
            if (spawned.TryGetValue(key, out var go))
            {
                Destroy(go);
                spawned.Remove(key);
            }
        }
    }

    void UpdateFor(ARTrackedImage img)
    {
        var key = img.referenceImage.name;

        // Create cube if we don't have one yet
        if (!spawned.TryGetValue(key, out var go))
        {
            go = Instantiate(cubePrefab, img.transform);
            go.transform.localPosition = Vector3.zero;            // sit on the image center
            go.transform.localRotation = Quaternion.identity;     // align with image
            spawned[key] = go;
        }

        // Show only while tracking is good
        bool visible = img.trackingState == TrackingState.Tracking;
        if (go.activeSelf != visible) go.SetActive(visible);
    }
}
