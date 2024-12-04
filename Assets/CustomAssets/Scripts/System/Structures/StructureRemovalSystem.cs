using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

partial struct StructureRemovalSystem : ISystem
{
    private ComponentLookup<HexTileData> tileDataLookup;
    private NativeParallelHashMap<int2, Entity> tileEntityMap;

    public void OnCreate(ref SystemState state)
    {
        tileDataLookup = state.GetComponentLookup<HexTileData>(false);
        tileEntityMap = new NativeParallelHashMap<int2, Entity>(0, Allocator.Persistent);
    }

    public void OnDestroy(ref SystemState state)
    {
        tileEntityMap.Dispose();
    }

    public void OnUpdate(ref SystemState state)
    {
        tileDataLookup.Update(ref state);

        // Rebuild tileEntityMap if necessary
        // Similar to BuildingSystem

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (structureData, entity) in SystemAPI.Query<StructureData>().WithEntityAccess())
        {
            if (!state.EntityManager.Exists(entity))
                continue;

            var buffer = state.EntityManager.GetBuffer<StructureOccupiedTile>(entity);

            // Mark tiles as unoccupied
            foreach (var tile in buffer)
            {
                int2 tileCoord = tile.TileCoordinate;
                if (tileEntityMap.TryGetValue(tileCoord, out Entity tileEntity))
                {
                    tileDataLookup[tileEntity] = new HexTileData
                    {
                        tileCoordinates = tileCoord,
                        isOccupied = false
                    };
                }
            }

            // Destroy the structure entity
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
