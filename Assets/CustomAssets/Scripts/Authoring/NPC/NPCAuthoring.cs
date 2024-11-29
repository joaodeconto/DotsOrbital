using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class NPCAuthoring : MonoBehaviour
{
    public Vector2 currentTile;
    public uint randomSeed;
    public float speed;

    public class Baker : Baker<NPCAuthoring>
    {
        public override void Bake(NPCAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NPCData
            {
                random = new Unity.Mathematics.Random(2),
                currentTile = int2.zero,
            });
            AddComponent(entity, new NPCMovement
            {
                isMoving = false,
                speed = authoring.speed,
            });
        }
    }
}

public struct NPCData : IComponentData
{
    public Unity.Mathematics.Random random;
    public int2 currentTile;
}

public struct NPCMovement : IComponentData
{
    public int2 targetTile;      // Coordinates of the target tile
    public float3 targetPosition; // World position of the target tile
    public float speed;          // Movement speed
    public bool isMoving;        // Whether the NPC is currently moving
}