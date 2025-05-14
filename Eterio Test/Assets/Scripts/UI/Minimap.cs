using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Minimap : MonoBehaviour
{
    public Camera miniMapCamera;
    public Camera mainCamera;
    public Transform modelBoundsRef;
    public float cameraHeight = 10f;
    public float smoothSpeed = 5f;

    private Vector3 targetPosition;
    private bool moving = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        Vector2 normalized = Rect.PointToNormalized(rect.rect, localCursor);
        Ray ray = miniMapCamera.ViewportPointToRay(normalized);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 moveTo = hit.point;
            moveTo.y = cameraHeight;

            Bounds bounds = new Bounds(modelBoundsRef.position, modelBoundsRef.localScale * 1.5f);
            moveTo.x = Mathf.Clamp(moveTo.x, bounds.min.x, bounds.max.x);
            moveTo.z = Mathf.Clamp(moveTo.z, bounds.min.z, bounds.max.z);

            targetPosition = moveTo;
            moving = true;
        }
    }

    void Update()
    {
        if (moving)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * smoothSpeed);

            if (Vector3.Distance(mainCamera.transform.position, targetPosition) < 0.1f)
                moving = false;
        }
    }
}
