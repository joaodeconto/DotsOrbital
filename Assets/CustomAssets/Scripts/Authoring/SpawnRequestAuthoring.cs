using Unity.Entities;

public struct SpawnRequest : IComponentData
{
    public Entity prefab;
}
