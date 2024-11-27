using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct AstroBodySpawnerSystem : ISystem
{    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<EntitiesReferences>(out EntitiesReferences entitiesReferences))
        {
            //UnityEngine.Debug.LogWarning("EntitiesReferences singleton not found. Skipping update.");
            return;
        }
        // Get the singleton for references (e.g., prefab entities)
        //EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        // Create an EntityCommandBuffer for spawning entities
        EntityCommandBuffer.ParallelWriter ecb =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // Prepare the job
        BodySpawnerJob job = new BodySpawnerJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            EntityPrefab = entitiesReferences.bodyPrefabEntity,
            CommandBuffer = ecb
        };

        // Schedule the job in parallel
        job.ScheduleParallel();
    }    
}

[BurstCompile]
public partial struct BodySpawnerJob : IJobEntity
{
    public float DeltaTime;
    public Entity EntityPrefab;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute(ref AstroBodySpawner bodySpawner, in LocalTransform localTransform)
    {
        // Update the spawn timer
        bodySpawner.timer -= DeltaTime;
        if (bodySpawner.timer > 0f)
            return;

        // Reset the timer and increment the spawned count
        bodySpawner.timer = bodySpawner.timerMax;
        bodySpawner.spawnedCount++;

        // Spawn a new entity and set its components
        Entity newEntity = CommandBuffer.Instantiate(bodySpawner.spawnedCount, EntityPrefab);
        CommandBuffer.SetComponent(bodySpawner.spawnedCount, newEntity, LocalTransform.FromPosition(localTransform.Position));
        CommandBuffer.AddComponent(bodySpawner.spawnedCount, newEntity, new RandomMovement
        {
            originPosition = localTransform.Position,
            targetPosition = localTransform.Position,
            distanceMin = bodySpawner.randomWalkingDistanceMin,
            distanceMax = bodySpawner.randomWalkingDistanceMax,
            random = new Unity.Mathematics.Random((uint)bodySpawner.spawnedCount + 1),
        });
    }
}