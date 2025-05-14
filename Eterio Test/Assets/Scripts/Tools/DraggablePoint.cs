using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggablePoint : MonoBehaviour
{
    private Camera mainCamera;

    private bool isSelected = false;
    private bool isDragging = false;

    private int markerLayer;

    private int layerMask;

    public Toggles toggles;

    private void Start()
    {
        mainCamera = Camera.main;
        markerLayer = LayerMask.NameToLayer("Marker");
        layerMask = ~(1 << markerLayer);
    }

    private void OnMouseDown()
    {
        isDragging = true;
    }

    private void OnMouseUp()
    {
        isDragging = false;
    }

    private void OnMouseEnter()
    {
        isSelected = true;
    }

    private void OnMouseExit()
    {
        isSelected = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (toggles.snapToEdge)
                    transform.position = SnapToNearestCorner(hit);
                else
                    transform.position = hit.point;
            }
        }
    }

    Vector3 SnapToNearestCorner(RaycastHit hit)
    {
        MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
        if (meshFilter == null) return hit.point;

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Transform modelTransform = meshFilter.transform;

        float closestDist = Mathf.Infinity;
        Vector3 closestPoint = hit.point;

        foreach (Vector3 v in vertices)
        {
            Vector3 worldV = modelTransform.TransformPoint(v);
            float dist = Vector3.Distance(hit.point, worldV);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestPoint = worldV;
            }
        }

        return closestPoint;
    }

}
