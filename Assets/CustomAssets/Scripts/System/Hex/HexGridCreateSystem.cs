using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct HexGridCreateSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EntitiesReferences>();
        state.RequireForUpdate<HexGridSizeData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();
        HexGridSizeData hexGridSizeData = SystemAPI.GetSingleton<HexGridSizeData>();

        Entity hexGridEntity = SystemAPI.GetSingletonEntity<EntitiesReferences>();

        if (hexGridSizeData.spawned)
            return;


        float3 gridPosition = SystemAPI.GetComponent<LocalTransform>(hexGridEntity).Position;
        float hexRadius = hexGridSizeData.radius;
        float hexWidth = hexRadius * 2f;
        float hexHeight = math.sqrt(3) * hexRadius;

        int gridWidth = hexGridSizeData.width;
        int gridHeight = hexGridSizeData.height;

        for (int r = 0; r < gridHeight; r++)
        {
            for (int q = 0; q < gridWidth; q++)
            {
                // Calculate position
                float x = hexWidth * (q + (r * 0.5f) - (r / 2));
                float y = 0;
                float z = hexHeight * r;

                float3 position = new float3(x, y, z) + gridPosition;

                // Create entity
                Entity hexTileEntity = state.EntityManager.Instantiate(entitiesReferences.hexTileEntity);

                LocalTransform hexTransform = new LocalTransform
                {
                    Position = position,
                    Scale = hexWidth - hexGridSizeData.offset,
                };

                HexTileData tileData = new HexTileData
                {
                    tileCoordinates = new int2(q, r),
                    isOccupied = false,
                };

                // Set position and data
                state.EntityManager.SetComponentData(hexTileEntity, hexTransform);
                state.EntityManager.SetComponentData(hexTileEntity, tileData);
            }
        }

        hexGridSizeData.spawned = true;
        SystemAPI.SetSingleton(hexGridSizeData);
    }

}
