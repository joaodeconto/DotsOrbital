using Unity.Entities;
using UnityEngine;
public class CannonController : MonoBehaviour
{
    [SerializeField] private Transform cannonBarrel; // Reference to the cannon barrel for rotation
    [SerializeField] private float rotationSpeed = 5f; // Speed of the rotation
    [SerializeField] private float zDistanceFromCamera = 10f; // How far the target is from the camera in world space

    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Vector3 cannonDirection;
    [SerializeField] private float cannonBallForce;
    private void Update()
    {
        RotateCannonUsingViewport();

        if (Input.GetMouseButtonDown(0))
        {
            ShootCannon();
        }
    }

    private void ShootCannon()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        EntityQuery entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        EntitiesReferences entitiesReferences = entityQuery.GetSingleton<EntitiesReferences>();

        Entity ent = entityManager.Instantiate(entitiesReferences.cannonBallEntity);
        var shootData = entityManager.GetComponentData<CannonShoot>(ent);
        shootData.direction = cannonDirection;
        shootData.force = cannonBallForce;
        shootData.spawnPosition = spawnPosition.position;
        shootData.OnShootCannonBall = true;
        entityManager.SetComponentData(ent, shootData);
    }

    private void RotateCannonUsingViewport()
    {
        // Get the mouse position in the viewport
        Vector3 mouseViewportPos = InputPosition.Instance.GetMouseScreenPostion();

        // Convert the viewport position to world space, keeping a fixed Z distance
        Vector3 targetWorldPosition = Camera.main.ViewportToWorldPoint(new Vector3(mouseViewportPos.x, mouseViewportPos.y, zDistanceFromCamera));

        // Calculate the direction from the cannon barrel to the target world position
        Vector3 direction = targetWorldPosition - cannonBarrel.position;

        // Flatten the direction vector (optional: depends on whether you need vertical rotation)
        direction.y = targetWorldPosition.y - cannonBarrel.position.y;

        // Ensure the direction has length to avoid NaN issues
        if (direction.sqrMagnitude > 0.01f)
        {
            cannonDirection = direction;

            // Calculate the target rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate the cannon towards the target rotation
            cannonBarrel.rotation = Quaternion.Lerp(cannonBarrel.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Debug for visualization
        //Debug.DrawRay(cannonBarrel.position, direction, Color.red);
        //Debug.Log("Viewport Position: " + mouseViewportPos + " | World Position: " + targetWorldPosition);
    }
}
