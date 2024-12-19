using UnityEngine;
using Unity.Entities;

public class MouseInputHandler : MonoBehaviour
{
    private EntityManager entityManager;
    private Entity mouseInputEntity;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create an entity with a MouseInput component
        mouseInputEntity = entityManager.CreateEntity(typeof(MouseInput));
    }

    void Update()
    {
        // Capture mouse position in world space
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Adjust if necessary

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 mouseWorldPosition = ray.GetPoint(enter);

            // Update the MouseInput component in ECS
            entityManager.SetComponentData(mouseInputEntity, new MouseInput
            {
                Position = mouseWorldPosition
                
            });
            //Debug.Log(mouseWorldPosition.ToString()+ " mouse posiyion");
        }
    }
}

