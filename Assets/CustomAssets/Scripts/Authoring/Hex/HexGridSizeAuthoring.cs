using Unity.Entities;
using UnityEngine;

public class HexGridSizeAuthoring : MonoBehaviour
{
    public int lines;
    public int columns;
    public float radius;
    public float offset;

    public class Baker : Baker<HexGridSizeAuthoring>
    {
        public override void Bake(HexGridSizeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HexGridSizeData
            {
                mapWidth = authoring.lines,
                mapHeight = authoring.columns,
                tileRadius = authoring.radius,
                tileOffset = authoring.offset,
            });
        }
    }
}

public struct HexGridSizeData : IComponentData
{
    public int mapWidth;
    public int mapHeight;
    public float tileRadius;
    public float tileOffset;
    public bool mapSpawned;
}