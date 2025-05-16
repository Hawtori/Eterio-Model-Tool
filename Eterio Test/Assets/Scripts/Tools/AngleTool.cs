using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AngleTool : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject pointMarkerPrefab;
    public LineRenderer lineAB;
    public LineRenderer lineCB;
    public LineRenderer arc;

    public MeshFilter arcMeshFilter;
    public float arcRadius = 0.2f;
    public int arcSegments = 75;

    public TMP_Text angleText;

    private List<Transform> selectedPoints = new List<Transform>();

    public Button pointSelectbtn;
    public Button anglebtn;

    public Toggles toggles;

    private void Start()
    {
        toggles.ChangeState("panning");
    }

    void Update()
    {
        // if we are checking for an angle currently
        if(toggles.GetCurrentState() == Toggles.States.angle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (selectedPoints.Count < 3)
                    {
                        GameObject point = Instantiate(pointMarkerPrefab, hit.point + hit.normal * toggles.pointOffset, Quaternion.identity, hit.transform);
                        selectedPoints.Add(point.transform);
                        if (selectedPoints.Count == 3)
                        {
                            UpdateAngleDisplay();
                        }
                    }
                }
            }
        }

        // update points if there are 3 active
        if (selectedPoints.Count == 3)
        {
            UpdateAngleDisplay();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            toggles.snapToEdge = !toggles.snapToEdge;
        }
    }

    void UpdateAngleDisplay()
    {
        Vector3 A = selectedPoints[0].position;
        Vector3 B = selectedPoints[1].position;
        Vector3 C = selectedPoints[2].position;

        Vector3 BA = (A - B).normalized;
        Vector3 BC = (C - B).normalized;

        float angle = Vector3.Angle(BA, BC);

        pointSelectbtn.gameObject.SetActive(true);
        anglebtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50f, 115f);

        lineAB.gameObject.SetActive(true);
        lineCB.gameObject.SetActive(true);
        lineAB.SetPosition(0, B);
        lineAB.SetPosition(1, A);

        lineCB.SetPosition(0, B);
        lineCB.SetPosition(1, C);

        arc.gameObject.SetActive(true);
        DrawArcLine(B, BA, BC, angle);
        DrawArc(B, BA, BC, angle);

        angleText.gameObject.SetActive(true);
        angleText.text = $"{angle:F1}°";

        Vector3 arcMidDirection = ((BA + BC) * 0.5f).normalized;
        Vector3 arcNormal = Vector3.Cross(BA, BC).normalized;
        Vector3 offset = arcMidDirection * 0.3f + arcNormal * 0.05f;

        angleText.transform.position = B + offset;
    }

    void DrawArcLine(Vector3 center, Vector3 dir1, Vector3 dir2, float angle)
    {
        int segments = 100;
        arc.positionCount = segments + 1;

        Vector3 normal = Vector3.Cross(dir1, dir2).normalized;
        Quaternion rotation = Quaternion.AngleAxis(0, normal);
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float currentAngle = t * angle;
            rotation = Quaternion.AngleAxis(currentAngle, normal);
            Vector3 pointOnArc = center + rotation * dir1 * arcRadius;
            arc.SetPosition(i, pointOnArc);
        }
    }

    void DrawArc(Vector3 center, Vector3 dir1, Vector3 dir2, float angle)
    {
        Vector3 normal = Vector3.Cross(dir1, dir2).normalized;

        Quaternion GetRotation(float a) => Quaternion.AngleAxis(a, normal);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i <= arcSegments; i++)
        {
            float t = (float)i / arcSegments;
            float currentAngle = t * angle;

            Vector3 point = center + GetRotation(currentAngle) * dir1.normalized * arcRadius;
            vertices.Add(point);
        }

        // Check if the triangle is back-facing to the camera
        bool flipTriangles = Vector3.Dot(normal, Camera.main.transform.forward) > 0;

        for (int i = 1; i < vertices.Count - 1; i++)
        {
            if (flipTriangles)
            {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i);
            }
            else
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
        }

        Mesh arcMesh = new Mesh();
        arcMesh.SetVertices(vertices);
        arcMesh.SetTriangles(triangles, 0);
        arcMesh.RecalculateNormals();
        arcMeshFilter.mesh = arcMesh;
    }

    public void ClearPoints()
    {
        foreach (Transform t in selectedPoints)
        {
            Destroy(t.gameObject);
        }
        selectedPoints.Clear();
        lineAB.SetPosition(0, Vector3.zero);
        lineAB.SetPosition(1, Vector3.zero);
        lineCB.SetPosition(0, Vector3.zero);
        lineCB.SetPosition(1, Vector3.zero);
        arc.positionCount = 0;

        lineAB.gameObject.SetActive(false);
        lineCB.gameObject.SetActive(false);
        angleText.gameObject.SetActive(false);
        arc.gameObject.SetActive(false);
        arcMeshFilter.mesh = null;
        angleText.text = "";


        pointSelectbtn.gameObject.SetActive(false);
        anglebtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50f, 62f);

    }

    public int GetNumberOfPoints() => selectedPoints.Count;
}  
