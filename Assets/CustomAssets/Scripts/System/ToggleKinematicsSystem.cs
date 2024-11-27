using Unity.Entities;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Burst;

public partial struct ToggleKinematicsSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GameOptions>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton(out GameOptions gameOptions))
            return;

        bool toggleAllKinematic = gameOptions.OnPhysicsToggle;

        if (toggleAllKinematic)
        {
            ToggleAllKinematicJob job = new ToggleAllKinematicJob
            {
                isKinematic = toggleAllKinematic,
            };

            job.ScheduleParallel();
            return;
        }

        foreach (RefRO<BodyPhysicsData> bodydate in SystemAPI.Query<RefRO<BodyPhysicsData>>())
        {
            if (bodydate.ValueRO.OnRequiredUpdate)
            {
                ToggleUpdateRequiredJob job = new ToggleUpdateRequiredJob();
                job.ScheduleParallel();
                return;
            }
        }      
    }
}

[BurstCompile]
public partial struct ToggleUpdateRequiredJob : IJobEntity
{
    public void Execute(
        ref BodyPhysicsData bodyData,
        ref PhysicsVelocity velocity,
        ref PhysicsMass mass)
    {
        if (!bodyData.OnRequiredUpdate)
            return;

        if (bodyData.IsDynamic)
        {
            // Set entity to "kinematic-like" by freezing velocity and mass
            velocity.Linear = float3.zero;
            velocity.Angular = float3.zero;

            // Zero out the inverse mass and inertia tensors
            mass.InverseMass = .1f;
            mass.InverseInertia = float3.zero;

            bodyData.IsDynamic = false;
        }
        else
        {
            // Restore dynamic behavior by resetting the mass properties
            mass.InverseMass = 1f; // Replace with the original inverse mass
            mass.InverseInertia = new float3(3f, 3f, 3f); // Replace with original inertia

            bodyData.IsDynamic = true;
        }

        bodyData.OnRequiredUpdate = false;
    }
}

[BurstCompile]
public partial struct ToggleAllKinematicJob : IJobEntity
{
    public bool isKinematic;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    public void Execute(
        ref BodyPhysicsData bodyData, 
        ref PhysicsVelocity velocity,
        ref PhysicsMass mass)
    {
            
        if (bodyData.IsDynamic)
        {
            // Set entity to "kinematic-like" by freezing velocity and mass
            velocity.Linear = float3.zero;
            velocity.Angular = float3.zero;

            // Zero out the inverse mass and inertia tensors
            mass.InverseMass = 0f;
            mass.InverseInertia = float3.zero;

            bodyData.IsDynamic = false;
        }
        else
        {
            // Restore dynamic behavior by resetting the mass properties
            mass.InverseMass = 1f; // Replace with the original inverse mass
            mass.InverseInertia = new float3(1f, 1f, 1f); // Replace with original inertia

            bodyData.IsDynamic = true;
        }
            
    }
}

