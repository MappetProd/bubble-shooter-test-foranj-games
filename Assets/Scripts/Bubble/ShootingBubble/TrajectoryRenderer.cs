using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using Color = UnityEngine.Color;

public class TrajectoryRenderer : MonoBehaviour
{

    LineRenderer[] lineRenderers;
    PlayerInput input;

    [SerializeField]
    private Material lineMaterial;

    Vector3[] vertexes;

    private Color ratioLinesColor;
    private void Awake()
    {
        lineRenderers = new LineRenderer[5];
        ratioLinesColor = Color.red;
        input = GetComponent<PlayerInput>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < lineRenderers.Length; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(gameObject.transform);
            lineRenderers[i] = obj.AddComponent<LineRenderer>();
        }
    }

    private List<Vector3> DrawLine(Vector2 _shotDirection, Color color, LineRenderer lr)
    {
        //Debug.DrawRay(transform.position, input.shotDirection, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _shotDirection, Mathf.Infinity, LayerMask.GetMask("Default"), Mathf.NegativeInfinity, Mathf.Infinity);

        List<Vector3> vertexes = new List<Vector3>();
        vertexes.Add(transform.position);

        Vector2 shotDirection = _shotDirection;
        int count = 0;
        while (hit.transform != null && hit.transform.gameObject.CompareTag("Wall") && count < 2)
        {
            vertexes.Add(hit.point);
            if (Vector2.Distance(shotDirection, Vector2.Reflect(shotDirection, hit.normal)) > 0.01f)
            {
                shotDirection = Vector2.Reflect(shotDirection, hit.normal);
                //Debug.DrawRay(hit.point, shotDirection, Color.red);
                hit = Physics2D.Raycast(hit.point + shotDirection * 0.01f, shotDirection, Mathf.Infinity, LayerMask.GetMask("Default"), Mathf.NegativeInfinity, Mathf.Infinity);
            }
            count++;
        }

        if (hit.transform != null && hit.transform.gameObject.CompareTag("Bubble"))
            vertexes.Add(hit.point);

        SetLineRenderer(lr, color, vertexes);
        return vertexes;
    }

    private void SetLineRenderer(LineRenderer lr, Color color, List<Vector3> vertexes)
    {
        lr.startColor = color;
        lr.endColor = color;
        lr.positionCount = vertexes.Count;
        lr.SetPositions(vertexes.ToArray());
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = lineMaterial;
    }

    private Vector3 GetIntersection(Vector3 A, Vector3 a, Vector3 B, Vector3 b)
    {
        Vector2 p1 = new Vector2(A.x, A.y);
        Vector2 p2 = new Vector2(a.x, a.y);

        Vector2 p3 = new Vector2(B.x, B.y);
        Vector2 p4 = new Vector2(b.x, b.y);

        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

        float u_a = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
        float u_b = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

        float IntersectionX = p1.x + u_a * (p2.x - p1.x);
        float IntersectionY = p1.y + u_a * (p2.y - p1.y);
        Vector3 Intersection = new Vector2(IntersectionX, IntersectionY);

        return Intersection;
    }

    private void DrawRatioLines(Vector2 _shotDirection)
    {
        //Debug.DrawRay(transform.position, _shotDirection, Color.yellow);
        float angle = 20f;

        Vector2 bottomTrajectoryRangeLine = Quaternion.Euler(0f, 0f, angle) * _shotDirection;
        Vector2 topTrajectoryRangeLine = Quaternion.Euler(0f, 0f, -angle) * _shotDirection;
        bottomTrajectoryRangeLine = bottomTrajectoryRangeLine.normalized;
        topTrajectoryRangeLine = topTrajectoryRangeLine.normalized;

        List<Vector3> vertexes1 = new List<Vector3>() { transform.position };
        List<Vector3> vertexes2 = new List<Vector3>() { transform.position };
        List<Vector3> vertexes3 = new List<Vector3>();

        RaycastHit2D lr1_lastHit = Physics2D.Raycast(transform.position, bottomTrajectoryRangeLine, Mathf.Infinity, LayerMask.GetMask("Default"), Mathf.NegativeInfinity, Mathf.Infinity);

        if (lr1_lastHit.transform != null)
        {
            vertexes1.Add(lr1_lastHit.point);
        }

        RaycastHit2D lr2_lastHit = Physics2D.Raycast(transform.position, topTrajectoryRangeLine, Mathf.Infinity, LayerMask.GetMask("Default"), Mathf.NegativeInfinity, Mathf.Infinity);

        if (lr2_lastHit.transform != null)
        {
            vertexes2.Add(lr2_lastHit.point);
            if (lr2_lastHit.transform.gameObject.CompareTag("Bubble"))
            {
                
            }
            /*else if (lr2_lastHit.transform.gameObject.CompareTag("Wall"))
            {
                bottomTrajectoryRangeLine = Vector2.Reflect(bottomTrajectoryRangeLine, lr1_lastHit.normal);
                RaycastHit2D test = lr1_lastHit;
                lr1_lastHit = Physics2D.Raycast(lr1_lastHit.point + bottomTrajectoryRangeLine * 0.01f, bottomTrajectoryRangeLine, Mathf.Infinity, LayerMask.GetMask("Default"), Mathf.NegativeInfinity, Mathf.Infinity);

                Vector3? intersection = GetIntersection(test.point, lr1_lastHit.point, transform.position, lr2_lastHit.point);
                if (intersection != null)
                    vertexes2.Add((Vector3)intersection);

                if (lr1_lastHit.transform != null && lr1_lastHit.transform.gameObject.CompareTag("Wall"))
                {
                    vertexes2.Add(lr1_lastHit.point);
                }

                vertexes3.Add(lr2_lastHit.point);
                topTrajectoryRangeLine = Vector2.Reflect(topTrajectoryRangeLine, lr2_lastHit.normal);
                RaycastHit2D lr3_lastHit = Physics2D.Raycast(lr2_lastHit.point + topTrajectoryRangeLine * 0.01f, topTrajectoryRangeLine, Mathf.Infinity, LayerMask.GetMask("Default"), Mathf.NegativeInfinity, Mathf.Infinity);
                vertexes3.Add(lr3_lastHit.point);
            }*/
        }

        SetLineRenderer(lineRenderers[0], ratioLinesColor, vertexes1);
        SetLineRenderer(lineRenderers[1], ratioLinesColor, vertexes2);
        SetLineRenderer(lineRenderers[2], ratioLinesColor, vertexes3);
    }

    // Update is called once per frame
    void Update()
    {
        DrawLine(input.shotDirection.normalized, Color.white, lineRenderers[4]);
        //Debug.DrawRay(transform.position, input.shotDirection, Color.red);
        if (input.pullPower == input.maxPullPower)
        {
            ToggleRatioLines(true);
            DrawRatioLines(input.shotDirection);
        }
        else
            ToggleRatioLines(false);

    }

    private void ToggleRatioLines(bool value)
    {
        for (int i = 0; i < lineRenderers.Length - 1; i++)
        {
            lineRenderers[i].enabled = value;
        }
    }
    private void OnDisable()
    {
        LineRenderer[] lineRenderers = GetComponentsInChildren<LineRenderer>();
        foreach (LineRenderer lr in lineRenderers)
        {
            lr.gameObject.SetActive(false);
        }
    }
}
