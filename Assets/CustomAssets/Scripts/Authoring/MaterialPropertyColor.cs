using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

[MaterialProperty("_BaseColor")]
public struct MaterialPropertyColor : IComponentData
{
    public float4 Value; // RGBA color
}

public struct NeedsColorUpdate : IComponentData { }