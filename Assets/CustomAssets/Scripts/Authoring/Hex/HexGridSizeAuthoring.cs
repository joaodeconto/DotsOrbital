using Unity.Entities;
using UnityEngine;

public class HexGridSizeAuthoring : MonoBehaviour
{
    public int lines;
    public int columns;
    public float radio;
    public float offset;

    public class Baker : Baker<HexGridSizeAuthoring>
    {
        public override void Bake(HexGridSizeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HexGridSizeData
            {
                width = authoring.lines,
                height = authoring.columns,
                radius = authoring.radio,
                offset = authoring.offset,
            });
        }
    }
}

public struct HexGridSizeData : IComponentData
{
    public int width;
    public int height;
    public float radius;
    public float offset;
    public bool spawned;
}