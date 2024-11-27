using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class BodyPhysicsDataAuthoring : MonoBehaviour
{
    public bool OnRequiredUpdate;
    public bool isDynamic;

     public class Baker : Baker<BodyPhysicsDataAuthoring>
    {
        public override void Bake(BodyPhysicsDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BodyPhysicsData
            {
                OnRequiredUpdate = authoring.OnRequiredUpdate,
                IsDynamic = authoring.isDynamic,
            });
        }
    }
}


public struct BodyPhysicsData : IComponentData
{
    public bool OnRequiredUpdate;
    public bool IsDynamic; // Determines if the body starts as dynamic or static
    public float3 velocity;
    public float3 angularVelocity;
    public float3 inverseMass;
    public float3 inverseInertia;
}
