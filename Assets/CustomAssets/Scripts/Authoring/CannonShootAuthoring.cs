using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CannonShootAuthoring : MonoBehaviour
{
    public float force;
    public Vector3 direction;
    public Vector3 spawnPosition;

    public class Baker : Baker<CannonShootAuthoring>
    {
        public override void Bake(CannonShootAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CannonShoot
            {
                force = authoring.force,
                direction = authoring.direction,
                spawnPosition = authoring.spawnPosition,
                OnShootCannonBall = false
            });
        }
    }
}

public struct CannonShoot : IComponentData
{

    public float force;
    public float3 direction;
    public float3 spawnPosition;
    public bool OnShootCannonBall;
}