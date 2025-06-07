using UnityEngine;
using UnityEngine.UI.Extensions;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages the creation, updating, and logic of all connector lines in the scene.
/// This is a singleton-like manager that is controlled by LineConnector instances.
/// </summary>
public class LineManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject linePrefab;

    // State Variables
    private UILineRenderer currentLine;
    private RectTransform canvasRectTransform;
    private bool dropWasSuccessful;

    // Properties
    public Dictionary<Transform, Transform> Connections { get; private set; } = new Dictionary<Transform, Transform>();
    
    /// <summary>
    /// A helper component attached to line instances to store their connection data.
    /// </summary>
    private class LineData : MonoBehaviour
    {
        public Transform startPoint;
        public Transform endPoint;
    }

    private void Awake()
    {
        canvasRectTransform = GetComponentInParent<Canvas>().transform as RectTransform;
    }

    #region Public API Methods

    /// <summary>
    /// Instantiates a new line starting at a given connection point.
    /// Called from LineConnector.OnBeginDrag.
    /// </summary>
    public void StartLine(Transform startPoint)
    {
        if (currentLine != null)
        {
            Destroy(currentLine.gameObject);
        }

        dropWasSuccessful = false;

        GameObject newLineObj = Instantiate(linePrefab, canvasRectTransform);
        currentLine = newLineObj.GetComponent<UILineRenderer>();

        var lineData = newLineObj.AddComponent<LineData>();
        lineData.startPoint = startPoint;

        Vector2 startPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, startPoint.position, null, out startPosition);
        currentLine.Points = new Vector2[] { startPosition, startPosition };
    }

    /// <summary>
    /// Updates the end point of the line-in-progress to follow the mouse.
    /// Called from LineConnector.OnDrag.
    /// </summary>
    public void UpdateLine(Vector2 mousePosition)
    {
        if (currentLine != null)
        {
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, mousePosition, null, out localMousePosition);
            currentLine.Points[1] = localMousePosition;
            currentLine.SetAllDirty();
        }
    }

    /// <summary>
    /// Finalizes a line connection if dropped on a valid target.
    /// Called from LineConnector.OnDrop.
    /// </summary>
    public void ConnectLine(GameObject droppedOnObject)
    {
        if (currentLine == null) return;

        Transform targetTransform = droppedOnObject.transform;
        // Ascend the hierarchy if the dropped object is a child (e.g., Text on a Button).
        if (targetTransform.GetComponent<LineConnector>() == null && targetTransform.parent != null)
        {
            targetTransform = targetTransform.parent;
        }

        if (targetTransform.TryGetComponent<LineConnector>(out LineConnector endConnector))
        {
            Transform endPoint = endConnector.transform.Find("ConnectionPoint");
            Transform startPoint = currentLine.GetComponent<LineData>().startPoint;

            if (endPoint != null && startPoint != endPoint && !IsPointConnected(endPoint))
            {
                dropWasSuccessful = true;

                Vector2 localEndPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, endPoint.position, null, out localEndPoint);
                currentLine.Points[1] = localEndPoint;
                currentLine.SetAllDirty();

                var lineData = currentLine.GetComponent<LineData>();
                lineData.endPoint = endPoint;
                
                Connections.Add(startPoint, endPoint);
                currentLine = null;
            }
        }
    }

    /// <summary>
    /// Cleans up a line if the drag ends without a successful connection.
    /// Called from LineConnector.OnEndDrag.
    /// </summary>
    public void EndLine()
    {
        if (!dropWasSuccessful && currentLine != null)
        {
            Destroy(currentLine.gameObject);
        }
        currentLine = null;
    }

    /// <summary>
    /// Clears all completed lines from the screen and the connections dictionary.
    /// Called by GameManager when a round ends or is reset.
    /// </summary>
    public void ClearAllLines()
    {
        LineData[] allLines = FindObjectsOfType<LineData>();
        foreach (var line in allLines)
        {
            Destroy(line.gameObject);
        }
        Connections.Clear();
    }
    
    /// <summary>
    /// Breaks a connection associated with a given point.
    /// Called by LineConnector.OnPointerDown.
    /// </summary>
    public void TryBreakConnection(Transform pointToCheck)
    {
        var connectionPair = Connections.FirstOrDefault(p => p.Key == pointToCheck || p.Value == pointToCheck);

        if (connectionPair.Key != null) // Default for KeyValuePair is null if not found
        {
            Connections.Remove(connectionPair.Key);

            // Find and destroy the associated line GameObject
            LineData[] allLines = FindObjectsOfType<LineData>();
            foreach (var line in allLines)
            {
                if (line.startPoint == connectionPair.Key)
                {
                    Destroy(line.gameObject);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Checks if a connection point is already part of a completed line.
    /// </summary>
    public bool IsPointConnected(Transform point)
    {
        return Connections.ContainsKey(point) || Connections.ContainsValue(point);
    }
    
    #endregion
}