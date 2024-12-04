using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{    
    public GameObject bodyGameobject;
    public GameObject astroBodyGameObject;
    public GameObject cannonBallGameObject;
    public GameObject shootLightGameObject;
    public GameObject hexTileGameObject;
    public GameObject npcGameObject;
    public GameObject structureTest;
    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bodyGameobject, TransformUsageFlags.Dynamic),
                bodyPrefabEntity = GetEntity(authoring.astroBodyGameObject, TransformUsageFlags.Dynamic),
                cannonBallEntity = GetEntity(authoring.cannonBallGameObject, TransformUsageFlags.Dynamic),
                shootlightEntity = GetEntity(authoring.shootLightGameObject, TransformUsageFlags.Dynamic),
                hexTileEntity = GetEntity(authoring.hexTileGameObject, TransformUsageFlags.Dynamic),
                npcEntity = GetEntity(authoring.npcGameObject, TransformUsageFlags.Dynamic),
                structureTestEntity = GetEntity(authoring.structureTest, TransformUsageFlags.Dynamic),
            });;
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity bulletPrefabEntity;
    public Entity bodyPrefabEntity;
    public Entity cannonBallEntity;
    public Entity shootlightEntity;
    public Entity hexTileEntity;
    public Entity npcEntity;
    public Entity structureTestEntity;
}
