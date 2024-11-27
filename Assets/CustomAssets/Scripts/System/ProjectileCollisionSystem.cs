using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
partial struct ProjectileCollisionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Ensure the system updates after the simulation step
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;

        var collisionJob = new CannonballCollisionJob
        {
            PhysicsWorld = physicsWorld,
            CannonShoot = SystemAPI.GetComponentLookup<CannonShoot>(true),
            CommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        };

        // Schedule collision job using Unity's Physics system
        state.Dependency = collisionJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
    }
}

[BurstCompile]
struct CannonballCollisionJob : ICollisionEventsJob
{
    [ReadOnly] public PhysicsWorld PhysicsWorld;
    [ReadOnly] public ComponentLookup<CannonShoot> CannonShoot;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute(CollisionEvent collisionEvent)
    {
        Entity entityA = collisionEvent.EntityA;
        Entity entityB = collisionEvent.EntityB;

        bool isEntityACannonball = CannonShoot.HasComponent(entityA);
        bool isEntityBCannonball = CannonShoot.HasComponent(entityB);

        if (isEntityACannonball || isEntityBCannonball)
        {
            Entity cannonballEntity = isEntityACannonball ? entityA : entityB;

            // Destroy the cannonball
            CommandBuffer.DestroyEntity(0, cannonballEntity);

            // Additional logic: Apply effects or damage to the other entity
        }
    }
}
