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
    public float cameraHeight = 2f;
    public float smoothSpeed = 5f;

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

        if(moving)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPos, Time.deltaTime * smoothSpeed);

            if (Vector3.Distance(mainCamera.transform.position, targetPos) < targetThreshold) moving = false;
        }
    }

    private void UpdateViewRect()
    {
        Vector3[] frustumCorners = new Vector3[4];
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        float maxViewDistance = 1000f;
        Ray[] rays = new Ray[]
        {
            mainCamera.ViewportPointToRay(new Vector3(0, 0, 0)),
            mainCamera.ViewportPointToRay(new Vector3(0, 1, 0)), 
            mainCamera.ViewportPointToRay(new Vector3(1, 1, 0)), 
            mainCamera.ViewportPointToRay(new Vector3(1, 0, 0))  
        };

        for (int i = 0; i < 4; i++)
        {
            if (ground.Raycast(rays[i], out float enter))
                frustumCorners[i] = rays[i].GetPoint(enter);
            else
                frustumCorners[i] = mainCamera.transform.position; // fallback
        }

        Vector2[] rectPoints = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            Vector3 worldPoint = frustumCorners[i];
            Vector3 localPoint = WorldToMinimapLocal(worldPoint);
            rectPoints[i] = localPoint;
        }

        Vector2 min = rectPoints[0];
        Vector2 max = rectPoints[0];
        foreach (Vector2 v in rectPoints)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        Vector2 center = (min + max) / 2;
        Vector2 size = max - min;

        // Clamp size to not exceed minimap
        size = Vector2.Min(size, minimapRect.rect.size);

        viewRect.anchoredPosition = center;
        viewRect.sizeDelta = size;
        //Vector3 camPos = mainCamera.transform.position;
        //float camSize = cameraHeight;

        //Vector3 min = worldBounds.min;
        //Vector3 max = worldBounds.max;

        //float mapWidth = max.x - min.x;
        //float mapHeight = max.z - min.z;

        //float percentX = (camPos.x - min.x) / mapWidth;
        //float percentY = (camPos.z - min.z) / mapHeight;

        //Vector2 anchoredPos = new Vector2(percentX * minimapRect.rect.width, percentY * minimapRect.rect.height);
        //viewRect.anchoredPosition = anchoredPos;

        //float viewWidth = minimapRect.rect.width * (camSize / mapWidth);
        //float viewHeight = minimapRect.rect.height * (camSize / mapHeight);
        //viewRect.sizeDelta = new Vector2(viewWidth, viewHeight);
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
        cameraHeight = Mathf.Clamp(
        cameraHeight - eventData.scrollDelta.y,
        5f,
        CalculateMaxCameraHeightToStayWithinBounds()
        );

        Vector3 camPos = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3(camPos.x, cameraHeight, camPos.z);
    }

    float CalculateMaxCameraHeightToStayWithinBounds()
    {
        float mapWidth = worldBounds.size.x;
        float mapHeight = worldBounds.size.z;
        float camFOV = mainCamera.fieldOfView;

        float maxSize = Mathf.Min(mapWidth, mapHeight) * 0.9f;

        float maxHeight = maxSize / (2f * Mathf.Tan(Mathf.Deg2Rad * camFOV / 2f));
        return maxHeight;
    }

}
