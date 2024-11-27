using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct RandomMovementSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        RandomMovementJob job = new RandomMovementJob();

        job.ScheduleParallel();

        /*
        foreach((
            RefRW<RandomMovement> randomMovement,
            RefRW<UnitMover> unitMover,
            RefRW<LocalTransform> localTransform
            )
            in SystemAPI.Query<
                RefRW<RandomMovement>,
                RefRW<UnitMover>,
                RefRW<LocalTransform>>())
        {
            if(math.distance(localTransform.ValueRO.Position, randomMovement.ValueRO.targetPosition) < ShipMoverSystem.REACH_TARGET_POSITION_DISTANCE_SQR)
            {

                Random random = randomMovement.ValueRO.random;

                float3 randomDirection = new float3(random.NextFloat(-1f,1f), random.NextFloat(-1f, 1f), random.NextFloat(-1f,1f));
                randomDirection = math.normalize(randomDirection);

                randomMovement.ValueRW.targetPosition =
                    randomMovement.ValueRO.targetPosition +
                    randomDirection * random.NextFloat(randomMovement.ValueRO.distanceMin, randomMovement.ValueRO.distanceMax);

                randomMovement.ValueRW.random = random;
            }
            else
            {
                unitMover.ValueRW.targetPosition = randomMovement.ValueRO.targetPosition;
            }


        }
        */

    }
}

[BurstCompile]
public partial struct RandomMovementJob : IJobEntity
{
    public void Execute(ref RandomMovement randomMovement, ref UnitMover unitMover, in LocalTransform localTransform)
    {
        if (math.distance(localTransform.Position, randomMovement.targetPosition) < ShipMoverSystem.REACH_TARGET_POSITION_DISTANCE_SQR)
        {
            Random random = randomMovement.random;

            float3 randomDirection = new(
                random.NextFloat(-1f, 1f),
                random.NextFloat(-1f, 1f),
                random.NextFloat(-1f, 1f));

            randomDirection = math.normalize(randomDirection);
            randomMovement.targetPosition =
                        randomMovement.targetPosition +
                        (randomDirection *
                        random.NextFloat(randomMovement.distanceMin, randomMovement.distanceMax));

            randomMovement.random = random;
        }
        else
        {
            unitMover.targetPosition = randomMovement.targetPosition;
        }
    }
}
