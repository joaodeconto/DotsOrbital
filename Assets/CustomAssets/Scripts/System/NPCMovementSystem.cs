using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct NPCMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Schedule the NPC movement job
        var movementJob = new NPCDoMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        //state.Dependency = movementJob.Schedule(state.Dependency);
    }
}


[BurstCompile]
public partial struct NPCDoMovementJob : IJobEntity
{
    public float DeltaTime;

    public void Execute(ref NPCMovement movement, ref LocalTransform transform, ref NPCData npcData)
    {
        if (!movement.isMoving)
            return;

        float3 currentPosition = transform.Position;
        float3 targetPosition = movement.targetPosition;

        // Move towards the target position
        float3 direction = math.normalize(targetPosition - currentPosition);
        float distance = math.distance(currentPosition, targetPosition);

        if (distance > 0.1f)
        {
            transform.Position += direction * movement.speed * DeltaTime;
        }
        else
        {
            // Snap to the target position when close enough
            transform.Position = targetPosition;

            // Mark the movement as complete
            movement.isMoving = false;

            // Mark the tile as occupied
            npcData.currentTile = movement.targetTile;
        }
    }
}
