using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class UnitMoverAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    public class Baker : Baker<UnitMoverAuthoring>
    {
        public override void Bake(UnitMoverAuthoring authoring)
        {
            Entity ent = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(ent, new UnitMover
            {
                moveSpeed = authoring.moveSpeed,
                rotationSpeed = authoring.rotateSpeed
            });               
        }
    }
}

public struct UnitMover : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float3 targetPosition;
}
