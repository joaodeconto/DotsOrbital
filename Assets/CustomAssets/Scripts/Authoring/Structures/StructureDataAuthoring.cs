using Unity.Entities;
using Unity.Mathematics;

public struct StructureData : IComponentData
{
    public int2 Size; // Width and height in tiles
}

[InternalBufferCapacity(0)] // Adjust capacity based on expected number of tiles
public struct StructureOccupiedTile : IBufferElementData
{
    public int2 TileCoordinate;
}

public struct CleanupTag : IComponentData { }