using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct StructurePlacementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<HexGridSizeData>();
        state.RequireForUpdate<MouseInput>();
        state.RequireForUpdate<TileOccupancyData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    { 
        var tileRadius= SystemAPI.GetSingleton<HexGridSizeData>().tileRadius;
        var mouseInput = SystemAPI.GetSingleton<MouseInput>();
        var occupancyMap = SystemAPI.GetSingleton<TileOccupancyData>().OccupiedTiles;

        // Convert mouse position to hex coordinates
        int2 hexCoords = WorldToHex(mouseInput.Position, tileRadius);

        // Check if tile is within grid bounds
        if (!IsWithinGridBounds(hexCoords))
            return;

        // Check tile occupancy
        bool isOccupied = occupancyMap.TryGetValue(hexCoords, out bool occupied) && occupied;

        // Update placement visual entity

        foreach ((
            RefRO<PlacementVisualTag> visualTag,
            RefRW<PlacementVisualData> visualData,
            RefRW<LocalTransform> tagTransform)
            in SystemAPI.Query<RefRO<PlacementVisualTag>,
            RefRW<PlacementVisualData>,
            RefRW<LocalTransform>>())
        {
            tagTransform.ValueRW.Position = HexToWorld(hexCoords, tileRadius);
            visualData.ValueRW.IsValidPlacement = !isOccupied;

        }

        // Handle placement confirmation input
        if (Input.GetMouseButtonDown(0) && !isOccupied)
        {
            EntityCommandBuffer ecb = SystemAPI
             .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
             .CreateCommandBuffer(state.WorldUnmanaged);

            // Instantiate the structure entity
            var structurePrefab = SystemAPI.GetSingleton<EntitiesReferences>().structureTestEntity;
            Entity structureEntity = ecb.Instantiate(structurePrefab);

            // Set structure position
            ecb.SetComponent(structureEntity,
                new LocalTransform { Position = HexToWorld(hexCoords, tileRadius) });

            // Update occupancy map
            occupancyMap[hexCoords] = true;

            ecb.AsParallelWriter();
            //ecb.Dispose();
        }
    }

    private int2 WorldToHex(float3 position, float hexRadius)
    {
        float hexWidth = hexRadius * 2f;
        float hexHeight = Mathf.Sqrt(3f) * hexRadius;

        float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexRadius;
        float r = (2f / 3f * position.z) / hexRadius;
        return HexRound(q, r);
    }

    private int2 HexRound(float q, float r)
    {
        float x = q;
        float z = r;
        float y = -x - z;

        int rx = Mathf.RoundToInt(x);
        int ry = Mathf.RoundToInt(y);
        int rz = Mathf.RoundToInt(z);

        float x_diff = Mathf.Abs(rx - x);
        float y_diff = Mathf.Abs(ry - y);
        float z_diff = Mathf.Abs(rz - z);

        if (x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        return new int2(rx, rz);
    }

    private float3 HexToWorld(int2 hexCoords, float hexRadius)
    {
        float x = hexRadius * Mathf.Sqrt(3f) * (hexCoords.x + hexCoords.y / 2f);
        float z = hexRadius * 1.5f * hexCoords.y;
        return new float3(x, 0f, z);
    }

    private bool IsWithinGridBounds(int2 hexCoords)
    {
        // Implement grid bounds check based on your grid dimensions
        return true; // Replace with actual bounds check
    }
}

public struct PlacementVisualTag : IComponentData { }

public struct PlacementVisualData : IComponentData
{
    public bool IsValidPlacement;
}

