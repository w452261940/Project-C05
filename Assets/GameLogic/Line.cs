using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public EdgeCollider2D edgeCollider2D;
    public Rigidbody2D rb;

    [HideInInspector] public List<Vector2> points = new List<Vector2>();
    [HideInInspector] public int pointsCount = 0;

    private float pointMinDistance = 0.1f;
    private float circlecolliderRaduis;

    public Vector2 GetLastPoint()
    {
        return (Vector2)lineRenderer.GetPosition(pointsCount - 1);

    }

    public void UsePhysics(bool usePhysic)
    {
        rb.isKinematic = !usePhysic;
    }

    public void SetLineWidth(float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        edgeCollider2D.edgeRadius = width / 2f;
        circlecolliderRaduis = width / 2f;

    }

    public void AddPoint(Vector2 newPoint)
    {
        if (pointsCount >= 1 && Vector2.Distance(newPoint, GetLastPoint())< pointMinDistance)
        {
            return;
        }

        points.Add(newPoint);
        pointsCount++;

        CircleCollider2D circleCollider = this.gameObject.AddComponent<CircleCollider2D>();
        circleCollider.offset = newPoint;
        circleCollider.radius = circlecolliderRaduis;

        lineRenderer.positionCount = pointsCount;
        lineRenderer.SetPosition(pointsCount - 1, newPoint);

        if (pointsCount > 1)
            edgeCollider2D.points = points.ToArray();


    }

    public void SetPointMinDistance(float distance)
    {
        pointMinDistance = distance;
    }

}
