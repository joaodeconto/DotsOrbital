
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct SpawnOnTileSystem : ISystem
{
    private Random _random;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.HasSingleton<SpawnRequest>())
        {
            return;
        }
        double elapsedTime = SystemAPI.Time.ElapsedTime;
        _random = new Random((uint)(0.000001f + elapsedTime * 1000));

        NativeList<Entity> availableTiles = new NativeList<Entity>(Allocator.Temp);

        EntityCommandBuffer ecb =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<HexTileData> tileData,
            Entity entity)
            in SystemAPI.Query<
                RefRW<HexTileData>>()
                .WithEntityAccess())
        {
            if (!tileData.ValueRO.isOccupied)
            {
                availableTiles.Add(entity);
            }
        }

        if (availableTiles.Length == 0)
        {
            availableTiles.Dispose();
            return;
        }
        foreach ((
            RefRO<SpawnRequest> requestData,
            Entity requestEntity)
            in SystemAPI.Query<
                RefRO<SpawnRequest>>().WithEntityAccess())
        {
            // Randomly select a tile
            int randomIndex = _random.NextInt(1, availableTiles.Length);
            Entity chosenTile = availableTiles[randomIndex];
            HexTileData chosenTileData = SystemAPI.GetComponent<HexTileData>(chosenTile);
            float3 tilePosition = SystemAPI.GetComponent<LocalTransform>(chosenTile).Position;

            // Instantiate and configure NPC
            Entity npc = state.EntityManager.Instantiate(requestData.ValueRO.prefab);
            state.EntityManager.SetComponentData(npc, LocalTransform.FromPosition(tilePosition));

            ecb.SetComponent(npc, new NPCData
            {
                currentTile = chosenTileData.tileCoordinates,
            });

            ecb.SetComponent(chosenTile, new HexTileData
            {
                isOccupied = true,
            });

            // Remove the spawn request
            ecb.DestroyEntity(requestEntity);
        }

        availableTiles.Dispose();
    }
}
