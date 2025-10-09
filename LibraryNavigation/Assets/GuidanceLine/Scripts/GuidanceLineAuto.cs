using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GuidanceLine
{
    /// <summary>
    /// Automatic GuidanceLine that uses NavMesh to calculate paths at runtime.
    /// No manual checkpoint placement needed - automatically generates checkpoints
    /// that avoid walls and obstacles!
    /// </summary>
    public class GuidanceLineAuto : MonoBehaviour
    {
        [Header("Path Settings")]
        [Tooltip("Reference your player/start position here")]
        public Transform startPoint;

        [Tooltip("Reference the target location")]
        public Transform endPoint;

        [Tooltip("Distance between auto-generated checkpoints")]
        [SerializeField]
        private float checkpointSpacing = 2f;

        [Tooltip("Height offset for the line above the ground")]
        [SerializeField]
        private float lineHeightOffset = 0.1f;

        [Header("Line Visualization")]
        [Tooltip("Adjust line width as needed")]
        [SerializeField]
        private float lineWidth = 0.05f;

        [Tooltip("Adjust the number of points between checkpoints. More = smoother")]
        [SerializeField]
        private int pointsPerSegment = 20;

        [Tooltip("Distance to consider checkpoint as reached")]
        [SerializeField]
        private float checkpointDistanceThreshold = 1f;

        [Header("NavMesh Settings")]
        [Tooltip("Which NavMesh areas to use for pathfinding")]
        [SerializeField]
        private int navMeshAreaMask = NavMesh.AllAreas;

        [Tooltip("Update path when target moves more than this distance")]
        [SerializeField]
        private float targetMovementThreshold = 0.5f;

        [Tooltip("Force path recalculation after this many seconds (0 = never)")]
        [SerializeField]
        private float maxPathAge = 5f;

        [Header("Debug")]
        [Tooltip("Show debug spheres along the path")]
        public bool showDebugGizmos = true;

        [SerializeField]
        private float gizmoSphereRadius = 0.1f;

        // Private variables
        private LineRenderer lineRenderer;
        private NavMeshPath navMeshPath;
        private List<Vector3> currentCheckpoints = new List<Vector3>();
        private int currentCheckpointIndex = 0;
        private float lastPathUpdateTime;
        private bool pathCalculated = false;
        private Vector3 lastTargetPosition;
        private Vector3 lastStartPosition;

        void Start()
        {
            InitializeLine();
            
            // Wait a frame before calculating path to ensure NavMesh is loaded
            Invoke(nameof(CalculatePath), 0.1f);
        }

        void InitializeLine()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }

            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            navMeshPath = new NavMeshPath();
        }

        void Update()
        {
            if (!pathCalculated)
                return;

            // Check if reached destination
            if (endPoint != null)
            {
                float distanceToEnd = Vector3.Distance(startPoint.position, endPoint.position);
                if (distanceToEnd <= checkpointDistanceThreshold)
                {
                    Debug.Log("GuidanceLineAuto: Reached destination!");
                    EndLine();
                    return;
                }
            }

            // Check if we need to recalculate path (only when necessary!)
            bool needsRecalculation = false;
            
            // Recalculate if target moved significantly
            if (endPoint != null && Vector3.Distance(endPoint.position, lastTargetPosition) > targetMovementThreshold)
            {
                needsRecalculation = true;
                Debug.Log("GuidanceLineAuto: Target moved, recalculating path...");
            }
            
            // Recalculate if start moved significantly (player went off path)
            if (startPoint != null && Vector3.Distance(startPoint.position, lastStartPosition) > targetMovementThreshold * 2f)
            {
                // Check if we're still roughly on the path
                bool onPath = false;
                foreach (Vector3 checkpoint in currentCheckpoints)
                {
                    if (Vector3.Distance(startPoint.position, checkpoint) < checkpointDistanceThreshold * 2f)
                    {
                        onPath = true;
                        break;
                    }
                }
                
                if (!onPath)
                {
                    needsRecalculation = true;
                    Debug.Log("GuidanceLineAuto: Player deviated from path, recalculating...");
                }
                
                lastStartPosition = startPoint.position;
            }
            
            // Force recalculation if path is too old (safety fallback)
            if (maxPathAge > 0 && Time.time - lastPathUpdateTime > maxPathAge)
            {
                needsRecalculation = true;
                Debug.Log("GuidanceLineAuto: Path too old, refreshing...");
            }
            
            if (needsRecalculation)
            {
                CalculatePath();
                lastPathUpdateTime = Time.time;
                if (endPoint != null) lastTargetPosition = endPoint.position;
            }

            // Check if reached current checkpoint (removes walked sections)
            if (currentCheckpoints.Count > 0 && currentCheckpointIndex < currentCheckpoints.Count)
            {
                float distanceToCheckpoint = Vector3.Distance(startPoint.position, currentCheckpoints[currentCheckpointIndex]);
                if (distanceToCheckpoint <= checkpointDistanceThreshold)
                {
                    currentCheckpointIndex++;
                    // Debug.Log($"GuidanceLineAuto: Passed checkpoint {currentCheckpointIndex}/{currentCheckpoints.Count}");
                }
            }

            // Draw the line (automatically shortens as checkpoints are passed)
            DrawCurvedLine();
        }

        void CalculatePath()
        {
            if (startPoint == null || endPoint == null)
            {
                Debug.LogWarning("GuidanceLineAuto: Start or End point is null!");
                return;
            }

            // Sample NavMesh to check if points are valid
            NavMeshHit startHit, endHit;
            bool startOnNavMesh = NavMesh.SamplePosition(startPoint.position, out startHit, 5f, navMeshAreaMask);
            bool endOnNavMesh = NavMesh.SamplePosition(endPoint.position, out endHit, 5f, navMeshAreaMask);

            if (!startOnNavMesh || !endOnNavMesh)
            {
                Debug.LogError("GuidanceLineAuto: Start or End position is not on NavMesh!\n" +
                    $"Start on NavMesh: {startOnNavMesh}\n" +
                    $"End on NavMesh: {endOnNavMesh}\n" +
                    "Solution: Make sure your Ground has NavMeshSurface component and is baked!");
                pathCalculated = false;
                return;
            }

            // Calculate NavMesh path using the sampled positions
            bool pathFound = NavMesh.CalculatePath(
                startHit.position,
                endHit.position,
                navMeshAreaMask,
                navMeshPath
            );

            if (!pathFound || navMeshPath.status != NavMeshPathStatus.PathComplete)
            {
                string statusMsg = navMeshPath.status == NavMeshPathStatus.PathPartial 
                    ? "Partial path found - target may be blocked!" 
                    : "No path found!";
                Debug.LogError($"GuidanceLineAuto: Could not calculate complete path! {statusMsg}\n" +
                    "Solution: Check that walls are marked as obstacles and NavMesh is properly baked.");
                pathCalculated = false;
                return;
            }

            // Generate checkpoints from NavMesh path
            GenerateCheckpoints();
            pathCalculated = true;
            
            // Store positions to detect movement
            if (endPoint != null) lastTargetPosition = endPoint.position;
            if (startPoint != null) lastStartPosition = startPoint.position;
            
            Debug.Log($"GuidanceLineAuto: Path calculated successfully! ({navMeshPath.corners.Length} corners)");
        }

        void GenerateCheckpoints()
        {
            currentCheckpoints.Clear();
            currentCheckpointIndex = 0;

            if (navMeshPath.corners.Length < 2)
                return;

            // Add checkpoints at each corner and interpolate between them
            for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
            {
                Vector3 start = navMeshPath.corners[i];
                Vector3 end = navMeshPath.corners[i + 1];
                
                float segmentLength = Vector3.Distance(start, end);
                int numCheckpoints = Mathf.Max(1, Mathf.FloorToInt(segmentLength / checkpointSpacing));

                for (int j = 0; j < numCheckpoints; j++)
                {
                    float t = (float)j / numCheckpoints;
                    Vector3 checkpoint = Vector3.Lerp(start, end, t);
                    checkpoint.y += lineHeightOffset; // Lift above ground
                    currentCheckpoints.Add(checkpoint);
                }
            }

            // Always add the final endpoint
            Vector3 finalPoint = navMeshPath.corners[navMeshPath.corners.Length - 1];
            finalPoint.y += lineHeightOffset;
            currentCheckpoints.Add(finalPoint);

            Debug.Log($"GuidanceLineAuto: Generated {currentCheckpoints.Count} checkpoints");
        }

        void DrawCurvedLine()
        {
            if (currentCheckpoints.Count < 2)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            // Create array of points to draw (start + active checkpoints + end)
            List<Vector3> pathPoints = new List<Vector3>();
            pathPoints.Add(startPoint.position);

            // Add only checkpoints we haven't passed yet
            for (int i = currentCheckpointIndex; i < currentCheckpoints.Count; i++)
            {
                pathPoints.Add(currentCheckpoints[i]);
            }

            if (pathPoints.Count < 2)
            {
                EndLine();
                return;
            }

            // Calculate total points for smooth curve
            int segmentCount = pathPoints.Count - 1;
            int totalPoints = (pointsPerSegment - 1) * segmentCount + 1;
            Vector3[] allPoints = new Vector3[totalPoints];
            int index = 0;

            // Generate curved line using Catmull-Rom spline
            for (int i = 0; i < pathPoints.Count - 1; i++)
            {
                Vector3 p0 = i == 0 ? pathPoints[i] : pathPoints[i - 1];
                Vector3 p1 = pathPoints[i];
                Vector3 p2 = pathPoints[i + 1];
                Vector3 p3 = (i < pathPoints.Count - 2) ? pathPoints[i + 2] : pathPoints[i + 1];

                for (int j = 0; j < pointsPerSegment; j++)
                {
                    float t = (float)j / (pointsPerSegment - 1);
                    Vector3 pointOnCurve = CatmullRom(p0, p1, p2, p3, t);

                    if (index < allPoints.Length)
                    {
                        allPoints[index++] = pointOnCurve;
                    }
                }
            }

            // Update LineRenderer
            lineRenderer.positionCount = index;
            lineRenderer.SetPositions(allPoints);
        }

        Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            return 0.5f * (
                (2.0f * p1) +
                (-p0 + p2) * t +
                (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
                (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3);
        }

        void EndLine()
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
            Debug.Log("GuidanceLineAuto: Reached destination!");
            this.enabled = false;
        }

        void OnDrawGizmos()
        {
            if (!showDebugGizmos || lineRenderer == null)
                return;

            // Draw line points
            Gizmos.color = Color.green;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Gizmos.DrawWireSphere(lineRenderer.GetPosition(i), gizmoSphereRadius);
            }

            // Draw checkpoints
            Gizmos.color = Color.yellow;
            foreach (Vector3 checkpoint in currentCheckpoints)
            {
                Gizmos.DrawWireSphere(checkpoint, gizmoSphereRadius * 2f);
            }

            // Draw NavMesh path corners
            if (navMeshPath != null && navMeshPath.corners.Length > 0)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < navMeshPath.corners.Length; i++)
                {
                    Gizmos.DrawWireSphere(navMeshPath.corners[i], gizmoSphereRadius * 3f);
                    if (i < navMeshPath.corners.Length - 1)
                    {
                        Gizmos.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1]);
                    }
                }
            }
        }

        // Public methods for runtime control
        public void SetDestination(Vector3 newDestination)
        {
            if (endPoint == null)
            {
                GameObject endObj = new GameObject("EndPoint");
                endPoint = endObj.transform;
                endPoint.SetParent(transform);
            }
            endPoint.position = newDestination;
            CalculatePath();
        }

        public void SetStartPoint(Transform newStart)
        {
            startPoint = newStart;
            CalculatePath();
        }

        public bool HasReachedDestination()
        {
            return currentCheckpointIndex >= currentCheckpoints.Count;
        }

        public float GetDistanceToDestination()
        {
            if (currentCheckpoints.Count == 0)
                return 0f;

            float totalDistance = 0f;
            Vector3 currentPos = startPoint.position;

            for (int i = currentCheckpointIndex; i < currentCheckpoints.Count; i++)
            {
                totalDistance += Vector3.Distance(currentPos, currentCheckpoints[i]);
                currentPos = currentCheckpoints[i];
            }

            return totalDistance;
        }
    }
}
