using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct TileMovementDecisionSystem : ISystem
{
    private Unity.Mathematics.Random _random;
    private ComponentLookup<HexTileData> tileDataLookup;
    private ComponentLookup<LocalTransform> localTransformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Initialize _random outside Burst-compatible code
        _random = new Unity.Mathematics.Random((uint) 10);
        tileDataLookup = state.GetComponentLookup<HexTileData>(true);
        localTransformLookup = state.GetComponentLookup<LocalTransform>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        tileDataLookup.Update(ref state);
        localTransformLookup.Update(ref state);

        HexGridSizeData hexGridSizeData = SystemAPI.GetSingleton<HexGridSizeData>();
        EntityCommandBuffer ecb = SystemAPI
            .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        NativeArray<int2> eastEven = new NativeArray<int2>(HexGridUtils.EastWestEvenOffsets.Length, Allocator.TempJob);
        for (int i = 0; i < HexGridUtils.EastWestEvenOffsets.Length; i++)
        {
            eastEven[i] = HexGridUtils.EastWestEvenOffsets[i];
        }

        NativeArray<int2> eastOds = new NativeArray<int2>(HexGridUtils.EastWestOddOffsets.Length, Allocator.TempJob);
        for (int i = 0; i < HexGridUtils.EastWestOddOffsets.Length; i++)
        {
            eastOds[i] = HexGridUtils.EastWestOddOffsets[i];
        }

        NativeParallelHashMap<int2, Entity> tileEntityMap = new NativeParallelHashMap<int2, Entity>(hexGridSizeData.width * hexGridSizeData.height, Allocator.TempJob);

        var buildMapJob = new BuildTileEntityMapJob
        {
            TileEntityMap = tileEntityMap.AsParallelWriter()
        };
        state.Dependency = buildMapJob.ScheduleParallel(state.Dependency);

        var movementJob = new NPCMovementJob
        {
            TileEntityMap = tileEntityMap,
            HexGridSizeData = hexGridSizeData,
            TileDataLookup = tileDataLookup,
            LocalTransformLookup = localTransformLookup,
            eastEven = eastEven,
            eastOds = eastOds,
            RandomGenerator = _random,
            Ecb = ecb.AsParallelWriter()
        };

        state.Dependency = movementJob.ScheduleParallel(state.Dependency);
        state.Dependency = tileEntityMap.Dispose(state.Dependency);
        state.Dependency = eastEven.Dispose(state.Dependency);
        state.Dependency = eastOds.Dispose(state.Dependency);
    }
}

[BurstCompile]
public partial struct NPCMovementJob : IJobEntity
{
    [ReadOnly] public NativeParallelHashMap<int2, Entity> TileEntityMap;
    [ReadOnly] public HexGridSizeData HexGridSizeData;
    [ReadOnly] public ComponentLookup<HexTileData> TileDataLookup;
    [ReadOnly] public ComponentLookup<LocalTransform> LocalTransformLookup;
    [NativeDisableParallelForRestriction] public NativeArray<int2> eastEven;
    [NativeDisableParallelForRestriction] public NativeArray<int2> eastOds;

    public Unity.Mathematics.Random RandomGenerator;
    public EntityCommandBuffer.ParallelWriter Ecb;

    public void Execute(ref NPCMovement npcMovement, in NPCData npcData, [EntityIndexInQuery] int entityIndex)
    {
        if (npcMovement.isMoving)
            return;

        int2 currentTile = npcData.currentTile;
        var isEven = currentTile.y % 2 == 0;
        var offsets = isEven ? eastEven : eastOds;

        NativeList<int2> neighbors = new NativeList<int2>(Allocator.Temp);

        for (int i = 0; i < offsets.Length; i++)
        {
            int2 neighbor = currentTile + offsets[i];
            if (neighbor.x >= 0 && neighbor.x <= HexGridSizeData.width &&
                neighbor.y >= 0 && neighbor.y <= HexGridSizeData.height)
            {
                neighbors.Add(neighbor);
            }
        }

        NativeList<int2> validTiles = new NativeList<int2>(Allocator.Temp);

        foreach (var neighbor in neighbors)
        {
            if (TileEntityMap.TryGetValue(neighbor, out Entity neighborEntity) &&
                TileDataLookup[neighborEntity].isOccupied == false)
            {
                validTiles.Add(neighbor);
            }
        }

        if (validTiles.Length == 0)
        {
            neighbors.Dispose();
            validTiles.Dispose();
            return;
        }

        int randomIndex = RandomGenerator.NextInt(0, validTiles.Length);
        int2 chosenTile = validTiles[randomIndex];

        if (TileEntityMap.TryGetValue(chosenTile, out Entity chosenTileEntity))
        {
            npcMovement.targetTile = chosenTile;

            if (LocalTransformLookup.TryGetComponent(chosenTileEntity, out LocalTransform localTransform))
            {
                npcMovement.targetPosition = localTransform.Position;
            }

            npcMovement.isMoving = true;

            Ecb.AddComponent<NeedsColorUpdate>(entityIndex, chosenTileEntity);
            Ecb.SetComponent(entityIndex, chosenTileEntity, new HexTileData
            {
                tileCoordinates = chosenTile,
                isOccupied = true
            });
        }

        neighbors.Dispose();
        validTiles.Dispose();
    }
}

[BurstCompile]
public partial struct BuildTileEntityMapJob : IJobEntity
{
    public NativeParallelHashMap<int2, Entity>.ParallelWriter TileEntityMap;

    public void Execute(Entity entity, in HexTileData tileData)
    {
        TileEntityMap.TryAdd(tileData.tileCoordinates, entity);
    }
}