using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour, IPointerClickHandler, IBeginDragHandler,IDragHandler, IScrollHandler
{
    [Header("Cameras")]
    public Camera minimapCamera;
    public Camera mainCamera;

    [Header("MiniMap UI")]
    public RectTransform viewRect;
    public RectTransform minimapRect;

    [Header("Model Bounds")]
    public LayerMask minimapLayer;
    private Bounds worldBounds;

    [Header("Movement")]
    public float cameraHeight = 4.5f;
    public float smoothSpeed = 5f;

    [Header("Zoom")]
    public float minFOV = 15f;
    public float maxFOV = 60f;

    private Vector3 targetPos;
    private bool moving = false;

    public float targetThreshold = 0.05f;

    private void Start()
    {
        worldBounds = CalculateWorldBounds();
        targetPos = mainCamera.transform.position;
    }

    private Bounds CalculateWorldBounds()
    {
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        Bounds bounds = new Bounds();

        bool hasInit = false;
        foreach (Renderer r in renderers)
        {
            if (((1 << r.gameObject.layer) & minimapLayer) == 0) continue;

            if (!hasInit)
            {
                bounds = r.bounds;
                hasInit = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        return bounds;
    }

    private void Update()
    {
        UpdateViewRect();

        if (moving)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, Time.deltaTime * smoothSpeed);

            if (Vector3.Distance(mainCamera.transform.position, targetPos) < targetThreshold) moving = false;
        }
    }

    private void UpdateViewRect()
    {
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Vector3[] worldCorners = new Vector3[4];
        Vector2[] minimapCorners = new Vector2[4];

        Vector3[] viewportCorners = new Vector3[]
        {
        new Vector3(0, 0), // Bottom-left
        new Vector3(0, 1), // Top-left
        new Vector3(1, 1), // Top-right
        new Vector3(1, 0)  // Bottom-right
        };

        for (int i = 0; i < 4; i++)
        {
            Ray ray = mainCamera.ViewportPointToRay(viewportCorners[i]);
            if (groundPlane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                worldCorners[i] = hitPoint;
                minimapCorners[i] = WorldToMinimapLocal(hitPoint);
            }
            else
            {
                // fallback: use camera position
                Vector3 fallback = mainCamera.transform.position;
                worldCorners[i] = fallback;
                minimapCorners[i] = WorldToMinimapLocal(fallback);
            }
        }

        // Calculate bounding box
        Vector2 min = minimapCorners[0];
        Vector2 max = minimapCorners[0];

        for (int i = 1; i < 4; i++)
        {
            min = Vector2.Min(min, minimapCorners[i]);
            max = Vector2.Max(max, minimapCorners[i]);
        }

        Vector2 size = max - min;
        Vector2 center = (min + max) * 0.5f;

        viewRect.anchoredPosition = center;
        viewRect.sizeDelta = size;
    }



    Vector2 WorldToMinimapLocal(Vector3 worldPoint)
    {
        Vector3 boundsMin = worldBounds.min;
        Vector3 boundsSize = worldBounds.size;

        float percentX = Mathf.InverseLerp(boundsMin.x, boundsMin.x + boundsSize.x, worldPoint.x);
        float percentY = Mathf.InverseLerp(boundsMin.z, boundsMin.z + boundsSize.z, worldPoint.z);

        return new Vector2(
            percentX * minimapRect.rect.width,
            percentY * minimapRect.rect.height
        );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, eventData.position, eventData.pressEventCamera, out Vector2 local))
        {
            MoveCameraTo(local);
        }
    }

    void MoveCameraTo(Vector2 localCursor)
    {
        Vector2 normalized = Rect.PointToNormalized(minimapRect.rect, localCursor);
        Ray ray = minimapCamera.ViewportPointToRay(normalized);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 moveTo = hit.point;
            moveTo.y = cameraHeight;

            moveTo.x = Mathf.Clamp(moveTo.x, worldBounds.min.x, worldBounds.max.x);
            moveTo.z = Mathf.Clamp(moveTo.z, worldBounds.min.z, worldBounds.max.z);

            targetPos = moveTo;
            moving = true;
        }
    }

    public void OnBeginDrag(PointerEventData eventData) => OnDrag(eventData);

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, eventData.position, eventData.pressEventCamera, out Vector2 local))
        {
            MoveCameraTo(local);
        }
    }

    public void OnScroll(PointerEventData eventData)
    {
        float newFOV = mainCamera.fieldOfView - eventData.scrollDelta.y * 2f;
        mainCamera.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
    }


}
