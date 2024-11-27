using Unity.Entities;
using UnityEngine;

public class GameOptionsAuthoring : MonoBehaviour
{
    public bool OnPhysicsToggle;

    public class Baker : Baker<GameOptionsAuthoring>
    {
        public override void Bake(GameOptionsAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new GameOptions
            {
                OnPhysicsToggle = authoring.OnPhysicsToggle,
            });
        }
    }
}

public struct GameOptions : IComponentData
{
    public bool OnPhysicsToggle;

}

