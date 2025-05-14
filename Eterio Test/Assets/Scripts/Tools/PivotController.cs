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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SetNewPivot(hit.point);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            dragStart = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }
        if (isDragging)
        {
            Vector3 dragDelta = Input.mousePosition - dragStart;
            float rotSpeed = 0.2f;

            transform.Rotate(Vector3.up, -dragDelta.x * rotSpeed, Space.World);
            transform.Rotate(Vector3.right, dragDelta.y * rotSpeed, Space.World);

            dragStart = Input.mousePosition;
        }
    }

    void SetNewPivot(Vector3 newPivot)
    {
        Transform model = transform.GetChild(0);
        Vector3 modelWorldPos = model.position;
        Quaternion modelWorldRot = model.rotation;

        transform.position = newPivot;

        model.position = modelWorldPos;
        model.rotation = modelWorldRot;

        if (currentMarker == null)
        {
            currentMarker = Instantiate(pivotMarkerPrefab);
        }
        currentMarker.transform.position = newPivot;
    }
}
