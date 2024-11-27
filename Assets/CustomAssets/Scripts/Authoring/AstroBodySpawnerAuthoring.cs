using Unity.Entities;
using UnityEngine;

public class AstroBodySpawnerAuthoring : MonoBehaviour
{
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int spawnedCount;

    public class Baker : Baker<AstroBodySpawnerAuthoring>
    {
        public override void Bake(AstroBodySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AstroBodySpawner
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                spawnedCount = 0,
            });;
        }
    }
}

public struct AstroBodySpawner : IComponentData
{
    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public int spawnedCount;
}