using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

public partial struct CubeSpawnSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRO<CubeSpawner> cubeSpawner in SystemAPI.Query<RefRO<CubeSpawner>>())
        {
            if (!cubeSpawner.ValueRO.spawned)
            {
                EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

                // Create an EntityCommandBuffer for spawning entities
                EntityCommandBuffer.ParallelWriter ecb =
                    SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                        .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

                // Prepare the job
                CubeSpawnJob job = new CubeSpawnJob
                {
                    EntityPrefab = entitiesReferences.bulletPrefabEntity,
                    CommandBuffer = ecb
                };

                job.ScheduleParallel();
                break;
            }
        }
    }
}

[BurstCompile]
public partial struct CubeSpawnJob : IJobEntity
{
    public Entity EntityPrefab;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute(
        Entity entity,
        in LocalTransform localTransform,
        ref CubeSpawner cubeSpawner,
        [EntityIndexInQuery] in int entityInQueryIndex)
    {
        if (cubeSpawner.spawned)
            return;

        for (int i = 0; i <= cubeSpawner.columns; i++)
        {
            for (int j = 0; j <= cubeSpawner.rows; j++)
            {
                for (int k = 0; k <= cubeSpawner.lines; k++)
                {
                    //Set InitialState
                    Entity newEntity =
                        CommandBuffer.Instantiate(i + j + k, EntityPrefab);

                    float3 offsetPosition = new(
                        cubeSpawner.offset.x * i,
                        cubeSpawner.offset.y * j,
                        cubeSpawner.offset.z * k
                        );

                    float3 globalTransform = offsetPosition + localTransform.Position;
                    
                    CommandBuffer.SetComponent(
                        i + j + k,
                        newEntity,
                        LocalTransform.FromPosition(globalTransform));

                    CollisionFilter filter = new CollisionFilter
                    {

                        BelongsTo = ~0u,
                        CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                        GroupIndex = 0

                    };

                    // Set to Toggle Dynamics
                    /*
                    BodyPhysicsData data = new BodyPhysicsData
                    {
                        OnRequiredUpdate = true,
                        IsDynamic = true
                    };

                    CommandBuffer.SetComponent(
                        i + j + k,
                        newEntity,
                        data
                        );
                    */
                }
            }
        }

        CommandBuffer.SetComponent(entityInQueryIndex, entity, new CubeSpawner
        {
            columns = cubeSpawner.columns,
            rows = cubeSpawner.rows,
            lines = cubeSpawner.lines,
            spawned = true
        });
    }
}


