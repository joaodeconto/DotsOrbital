using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public float mass = 1.0f; // Mass of the body
    public Vector3 initialVelocity; // Initial velocity of the body
    public Vector3 velocity; // Current velocity (updated at runtime)

    private void Start()
    {
        velocity = initialVelocity; // Initialize velocity
    }
}
