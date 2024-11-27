using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Physics;

[BurstCompile]
partial struct CannonShootSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var job = new CannonShootJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        state.Dependency = job.ScheduleParallel(state.Dependency);
    }
}

[BurstCompile]
public partial struct CannonShootJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref CannonShoot cannonShoot, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, ref PhysicsCollider collider)
    {
        if (!cannonShoot.OnShootCannonBall)
            return;

        // Reset the shooting flag
        cannonShoot.OnShootCannonBall = false;

        // Set cannonball position and direction
        localTransform.Position = cannonShoot.spawnPosition;
        localTransform.Rotation = quaternion.LookRotation(cannonShoot.direction, math.up());

        // Apply velocity to the cannonball
        physicsVelocity.Linear = math.normalize(cannonShoot.direction) * cannonShoot.force;

        // Note: Additional physics-related logic can be added here if needed
    }
}
