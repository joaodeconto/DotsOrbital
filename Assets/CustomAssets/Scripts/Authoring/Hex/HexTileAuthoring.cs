using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class HexTileAuthoring : MonoBehaviour
{
    public Color tileInitialColor = Color.blue;
    public class Baker : Baker<HexTileAuthoring>
    {
        public override void Bake(HexTileAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new HexTileData
            {
            });
            AddComponent(entity, new MaterialPropertyColor
            {
                Value = new float4(
                    authoring.tileInitialColor.r,
                    authoring.tileInitialColor.g,
                    authoring.tileInitialColor.b,
                    authoring.tileInitialColor.a
                    )
            });
        }
    }
}

public struct HexTileData : IComponentData
{
    public int2 tileCoordinates;
    public bool isOccupied;
}
