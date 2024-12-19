using Unity.Burst;
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
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var tileRadius = SystemAPI.GetSingleton<HexGridSizeData>().tileRadius;
        var mouseInput = SystemAPI.GetSingleton<MouseInput>();


        EntityCommandBuffer ecb = SystemAPI
        .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
        .CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<BuildingRequestData> requestData,
            RefRW<LocalTransform> requestTransform,
            Entity requestEntity)
            in SystemAPI.Query<
               RefRO<BuildingRequestData>,
               RefRW<LocalTransform>>().WithEntityAccess())
        {
            // Convert mouse position to hex coordinates
            int2 hexCoords = WorldToHex(mouseInput.Position, tileRadius);
            //Debug.Log("found builkding request " + hexCoords);
            // Check if tile is within grid bounds
            if (!IsWithinGridBounds(hexCoords))
                return;

            bool isOccupied = false;
            float3 tilePosition = new(0, 0, 0);
            foreach ((
                RefRO<LocalTransform> tileTransform,
                RefRO<HexTileData> tileData,
                Entity tileEntity)
                in SystemAPI.Query<
                    RefRO<LocalTransform>,
                    RefRO<HexTileData>>()
                    .WithEntityAccess())
            {
                if (math.all(tileData.ValueRO.tileCoordinates == hexCoords))
                {
                    isOccupied = tileData.ValueRO.isOccupied;
                    tilePosition += tileTransform.ValueRO.Position;
                    float3 tileCenterPosition = HexToWorld(hexCoords, tileRadius);
                    requestTransform.ValueRW.Position = tileCenterPosition;
                    requestTransform.ValueRW.Scale = tileRadius *2 ;


                    Debug.Log(tileData.ValueRO.tileCoordinates + " " + hexCoords + " position " + tilePosition);


                    //ecb.SetComponent(requestEntity, LocalTransform.FromPosition(tilePosition));
                    //ecb.SetComponent(requestEntity, LocalTransform.FromScale(tileRadius*2));
                    return;
                }
            }



            /*
            // Instantiate the structure entity
            var structurePrefab = SystemAPI.GetSingleton<EntitiesReferences>().structureTestEntity;
            Entity structureEntity = ecb.Instantiate(structurePrefab);
            */

            // Set structure position

            //ecb.AsParallelWriter();
            //ecb.Dispose();
        }

        /*
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
        */
    }

    private int2 WorldToHex(float3 position, float hexRadius)
    {
        float q = 2f / 3f * position.x / hexRadius;
        float r = ((-1f / 3f * position.x) + (math.sqrt(3f) / 3f * position.z)) / hexRadius;
        return HexRound(q, r);
    }

    private int2 HexRound(float q, float r)
    {
        float x = q;
        float z = r;
        float y = -x - z;

        int rx = (int)math.round(x);
        int ry = (int)math.round(y);
        int rz = (int)math.round(z);

        float x_diff = math.abs(rx - x);
        float y_diff = math.abs(ry - y);
        float z_diff = math.abs(rz - z);

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
        float x = hexRadius * 3f / 2f * hexCoords.x;
        float z = hexRadius * math.sqrt(3f) * (hexCoords.y + hexCoords.x / 2f);
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

