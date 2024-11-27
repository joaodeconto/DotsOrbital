using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

partial struct ColorChangeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
        // Process only tiles marked with NeedsColorUpdate
        foreach (var (
            tileData,
            tileColor,
            materialMesh,
            entity)
            in SystemAPI.Query<
                HexTileData,
                RefRW<MaterialPropertyColor>,
                MaterialMeshInfo>()
                .WithEntityAccess()
                .WithAll<NeedsColorUpdate>())
        {
            // Set color based on occupancy state
            float4 newColor = tileData.isOccupied ? new float4(1, 0, 0, 1) : new float4(0, 1, 0, 1);

            if (!tileColor.ValueRO.Value.Equals(newColor))
            {
                tileColor.ValueRW.Value = newColor;
            }

            // Remove the marker once processed
            ecb.RemoveComponent<NeedsColorUpdate>(entity);
        }
    }
}