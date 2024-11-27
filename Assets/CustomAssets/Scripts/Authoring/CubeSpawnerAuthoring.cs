using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class CubeSpawnerAuthoring : MonoBehaviour
{
    public int lines;
    public int columns;
    public int rows;
    public Vector3 offset;

    public class Baker : Baker<CubeSpawnerAuthoring>
    {
        public override void Bake(CubeSpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new CubeSpawner
            {
                lines = authoring.lines,
                columns = authoring.columns,
                rows = authoring.rows,
                spawned = false,
                offset = authoring.offset
            });
        }
    }
}

public struct CubeSpawner : IComponentData
{
    public int lines;
    public int columns;
    public int rows;
    public bool spawned;
    public float3 offset;
}