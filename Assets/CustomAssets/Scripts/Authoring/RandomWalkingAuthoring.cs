using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomWalkingAuthoring : MonoBehaviour
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMax;
    public float distanceMin;
    public uint randomSeed;

    public class Baker : Baker<RandomWalkingAuthoring>
    {
        public override void Bake(RandomWalkingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomMovement
            {
                targetPosition = authoring.targetPosition,
                originPosition = authoring.originPosition,
                distanceMax = authoring.distanceMax,
                distanceMin = authoring.distanceMin,
                random = new Unity.Mathematics.Random(authoring.randomSeed), 
            });
        }
    }
}

public struct RandomMovement : IComponentData
{
    public float3 targetPosition;
    public float3 originPosition;
    public float distanceMax;
    public float distanceMin;
    public Unity.Mathematics.Random random;

}
