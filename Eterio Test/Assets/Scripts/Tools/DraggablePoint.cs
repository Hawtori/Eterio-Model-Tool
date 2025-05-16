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
        if (toggles.GetCurrentState() != Toggles.States.move) return;

        if (isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                if (toggles.snapToEdge)
                    transform.position = SnapToNearestEdge(hit);
                else
                    transform.position = hit.point;
            }
        }
    }

    // if we wanted to snap it to a corner use this
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

    Vector3 SnapToNearestEdge(RaycastHit hit)
    {
        MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
        if(!meshFilter) return hit.point;

        Mesh mesh = meshFilter.sharedMesh;
        if(!mesh) return hit.point;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Transform modelTransform = meshFilter.transform;

        float closestDist = Mathf.Infinity;
        Vector3 closestPoint = hit.point;

        for(int i = 0; i < triangles.Length; i+= 3) 
        {
            Vector3[] edgePoints = new Vector3[]
            {
                vertices[triangles[i]],
                vertices[triangles[i + 1]],
                vertices[triangles[i + 2]]
            };

            CheckEdge(edgePoints[0], edgePoints[1]);
            CheckEdge(edgePoints[1], edgePoints[2]);
            CheckEdge(edgePoints[2], edgePoints[0]);
        }

        return closestPoint;
    
        void CheckEdge(Vector3 localA, Vector3 localB)
        {
            Vector3 worldA = modelTransform.TransformPoint(localA);
            Vector3 worldB = modelTransform.TransformPoint(localB);

            Vector3 projected = ProjectPointOnSegment(hit.point, worldA, worldB);
            float dist = Vector3.Distance(hit.point, projected);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestPoint = projected;
            }
        }
    }

    // helper for finding an edge
    Vector3 ProjectPointOnSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ab = b - a;
        float t = Vector3.Dot(point - a, ab.normalized) / ab.magnitude;

        t = Mathf.Clamp01(t);
        return a + t * ab;
    }

}
