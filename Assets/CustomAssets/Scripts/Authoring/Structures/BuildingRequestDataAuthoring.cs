using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public class BuildingRequestDataAuthoring : MonoBehaviour
{
    public Entity StructurePrefab;

    public class Baker : Baker<BuildingRequestDataAuthoring>
    {
        public override void Bake(BuildingRequestDataAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingRequestData
            {
                StructurePrefab = authoring.StructurePrefab,
                GridPosition = int2.zero,
                Size = 1
            }) ;
        }
    }
}
public struct BuildingRequestData : IComponentData
{
    public Entity StructurePrefab; // Prefab of the structure to build
    public int2 GridPosition;       // Desired grid position for the structure's origin
    public int2 Size;               // Size of the structure in tiles
}