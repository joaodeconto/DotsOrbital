using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitTrail : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private const int trailLength = 500;
    private Queue<Vector3> positions = new Queue<Vector3>();

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    private void Update()
    {
        // Add the current position to the trail
        if (positions.Count >= trailLength)
        {
            positions.Dequeue();
        }
        positions.Enqueue(transform.position);

        // Update the LineRenderer
        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
    }
}
