using UnityEngine;
using System.Collections.Generic;

public class GravitySimulator : MonoBehaviour
{
    public List<CelestialBody> bodies; // List of all celestial bodies
    public float gravitationalConstant = 1.0f; // Gravitational constant

    private void FixedUpdate()
    {
        SimulateGravity(Time.fixedDeltaTime);
    }

    private void SimulateGravity(float deltaTime)
    {
        // Loop through each pair of celestial bodies to calculate gravitational forces
        foreach (var bodyA in bodies)
        {
            Vector3 totalForce = Vector3.zero;

            foreach (var bodyB in bodies)
            {
                if (bodyA == bodyB) continue; // Skip self

                Vector3 direction = bodyB.transform.position - bodyA.transform.position;
                float distanceSquared = direction.sqrMagnitude;
                if (distanceSquared < Mathf.Epsilon) continue; // Avoid division by zero

                float forceMagnitude = (gravitationalConstant * bodyA.mass * bodyB.mass) / distanceSquared;
                totalForce += direction.normalized * forceMagnitude;
            }

            // Apply the total force to update velocity
            Vector3 acceleration = totalForce / bodyA.mass;
            bodyA.velocity += acceleration * deltaTime;
        }

        // Update positions based on velocity
        foreach (var body in bodies)
        {
            body.transform.position += body.velocity * deltaTime;
        }
    }
}
