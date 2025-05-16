using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotController : MonoBehaviour
{
    public Camera mainCamera;          
    public GameObject pivotMarkerPrefab;
    private GameObject currentMarker;

    private bool isDragging = false;
    private Vector3 dragStart;

    public Toggles toggles;

    private Vector3 startingPosition;

    private void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        // set a pivot point if we are setting a pivot currently
        if (toggles.GetCurrentState() == Toggles.States.pivot)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    SetNewPivot(hit.point);
                }
            }
            return;
        }

        // if we're not rotating, the rest of the function is not needed
        if (!toggles.IsInRotatingState()) return;
        
        // drag to rotate
        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            dragStart = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        // rotate the shape if we're dragging
        if (isDragging)
        {
            Vector3 dragDelta = Input.mousePosition - dragStart;
            float rotSpeed = 0.2f;

            switch(toggles.GetCurrentState())
            {
                case Toggles.States.rotatingY:
                    transform.Rotate(Vector3.right, dragDelta.y * rotSpeed, Space.World);
                    break;
                case Toggles.States.rotatingZ:
                    transform.Rotate(Vector3.up, -dragDelta.x * rotSpeed, Space.World);
                    break;
                case Toggles.States.rotatingX:
                    transform.Rotate(Vector3.forward, -dragDelta.x * rotSpeed, Space.World);
                    break;
            }

            dragStart = Input.mousePosition;
        }
    }

    public void RotateX90()
    {
        transform.Rotate(Vector3.forward, 90f, Space.World);
    }

    public void RotateY45()
    {
        transform.Rotate(Vector3.right, 45f, Space.World);
    }

    public void RotateZ45()
    {
        transform.Rotate(Vector3.up, 45f, Space.World);
    }

    void SetNewPivot(Vector3 newPivot)
    {
        Transform model = transform.GetChild(0);

        // save model's current position and rotation
        Vector3 modelWorldPos = model.position;
        Quaternion modelWorldRot = model.rotation;
        
        // move pivot root to new point
        transform.position = newPivot;

        // bring the model back to its original world position
        model.position = modelWorldPos;
        model.rotation = modelWorldRot;

        if (currentMarker == null)
        {
            currentMarker = Instantiate(pivotMarkerPrefab);
        }
        currentMarker.transform.position = newPivot;
    }

    public void ClearPivot()
    {
        if (currentMarker)
        {
            Destroy(currentMarker); 
            currentMarker = null;
        }

        GameObject angleTool = GameObject.Find("Angle tool manager");

        if(angleTool && angleTool.TryGetComponent(out AngleTool angles))
        {
            if(angles.GetNumberOfPoints() > 0)
                return;
            
        }

        Transform model = transform.GetChild(0);

        transform.position = startingPosition;
        transform.rotation = Quaternion.identity;

        model.position = Vector3.zero;
        model.rotation = Quaternion.identity;
    }
}
