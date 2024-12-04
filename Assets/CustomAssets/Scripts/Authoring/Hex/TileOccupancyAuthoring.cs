using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TileOccupancyAuthoring : MonoBehaviour
{
    public class Baker : Baker<TileOccupancyAuthoring>
    {
        public override void Bake(TileOccupancyAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TileOccupancyData
            {
            });
        }
    }

}

public struct TileOccupancyData : IComponentData
{
    public NativeHashMap<int2, bool> OccupiedTiles;
}

