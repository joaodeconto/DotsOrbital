using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

partial struct ShipMoverSystem : ISystem
{

    public const float REACH_TARGET_POSITION_DISTANCE_SQR = 1.2f;


    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        UnitMoverJob unitMoverJob = new UnitMoverJob 
        {
            deltaTime = SystemAPI.Time.DeltaTime 
        };

        unitMoverJob.ScheduleParallel();
        /*
        //Without Job
        foreach (
            (RefRW<LocalTransform> localTransform,
            RefRO<UnitMover> unitMover,
            RefRW<PhysicsVelocity> physicsVelocity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<UnitMover>,
                RefRW<PhysicsVelocity>
                >())
        {
            float3 moveDirection = unitMover.ValueRO.targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            localTransform.ValueRW.Rotation =
                math.slerp(
                    localTransform.ValueRO.Rotation,
                    quaternion.LookRotation(moveDirection, math.up()),
                    SystemAPI.Time.DeltaTime * unitMover.ValueRO.rotationSpeed);
            physicsVelocity.ValueRW.Linear = moveDirection * unitMover.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;

            //localTransform.ValueRW.Position = localTransform.ValueRO.Position 
            //    + new float3(moveSpeed.ValueRO.value, 0.0f, 0.0f) * SystemAPI.Time.DeltaTime;
        }
        */
    }
}

[BurstCompile]
public partial struct UnitMoverJob : IJobEntity 
{
    public float deltaTime;
    public void Execute(ref LocalTransform localTransform, in UnitMover unitMover,ref PhysicsVelocity physicsVelocity )
    {
        float3 moveDirection = unitMover.targetPosition - localTransform.Position;

        float reachedTargetDistanceSqr = ShipMoverSystem.REACH_TARGET_POSITION_DISTANCE_SQR;

        if (math.lengthsq(moveDirection) < reachedTargetDistanceSqr)
        {
            physicsVelocity.Linear = float3.zero;
            physicsVelocity.Angular = float3.zero;
            return;
        }

        moveDirection = math.normalize(moveDirection);

        localTransform.Rotation =
            math.slerp(
                localTransform.Rotation,
                quaternion.LookRotation(moveDirection, math.up()),

        deltaTime * unitMover.rotationSpeed);
        physicsVelocity.Linear = moveDirection * unitMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;

    }

} 
