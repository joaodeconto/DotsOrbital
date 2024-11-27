using Unity.Burst;
using Unity.Entities;

partial struct SpawnRequestSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (UnityEngine.Input.GetMouseButtonDown(1))
        {
            EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

            EntityCommandBuffer commandBuffer = SystemAPI
                .GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            Entity request = commandBuffer.CreateEntity();
            SpawnRequest spawnRequest = new SpawnRequest
            {
                prefab = entitiesReferences.npcEntity
            };

            commandBuffer.AddComponent(request,spawnRequest);
        }
    }
}
