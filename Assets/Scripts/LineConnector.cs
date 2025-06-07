using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attached to UI elements that can be connected by lines (pictures and name buttons).
/// It handles all user input events (clicks, drags) and delegates the logic to the LineManager.
/// </summary>
public class LineConnector : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Tooltip("The data asset associated with this specific connector.")]
    public QuizItem itemData;
    
    private LineManager lineManager;
    private Transform connectionPoint;

    private void Awake()
    {
        // Cache references for performance.
        lineManager = FindObjectOfType<LineManager>();
        connectionPoint = transform.Find("ConnectionPoint");
    }

    /// <summary>
    /// Called when the user first clicks down on this object.
    /// This is used to break an existing connection if one exists.
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (lineManager.IsPointConnected(connectionPoint))
        {
            lineManager.TryBreakConnection(connectionPoint);
            
            // Prevent the drag event from starting if we are breaking a line.
            eventData.pointerDrag = null; 
        }
    }

    /// <summary>
    /// Called when the user begins to drag this object, but only if a connection isn't being broken.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        lineManager.StartLine(connectionPoint);
    }

    /// <summary>
    /// Called every frame the user is dragging this object.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        lineManager.UpdateLine(eventData.position);
    }
    
    /// <summary>
    /// Called when the user releases the mouse button after a drag.
    /// Fired on the object that a drag *started* on.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        lineManager.EndLine();
    }
    
    /// <summary>
    /// Called when another draggable object is dropped onto this one.
    /// Fired on the object that a drag *ended* on.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        lineManager.ConnectLine(gameObject);
    }
}