using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;


public class StructureManager : MonoBehaviour
{
    [SerializeField] private Button structureBtn;
    [SerializeField] private int structureSize = 1;
    private bool isPlacingStructure;
    private EntityManager entityManager;
    private Entity structureEntityPrefab;
    private GameObject dummyPlacingObject;
    private float hexWidth;
    private float hexHeight;

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Retrieve the EntitiesReferences singleton
        var entitiesReferencesQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        var entitiesReferences = entitiesReferencesQuery.GetSingleton<EntitiesReferences>();

        structureEntityPrefab = entitiesReferences.structureTestEntity;
        structureBtn.onClick.AddListener(() => OnPlayerAttemptBuild());
    }

    private void InstantiateDummyStructure()
    {
        //dummyPlacingObject = Instantiate(dummyPrefab);
        isPlacingStructure = true;
    }

    private void Update()
    {
        if(isPlacingStructure)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPlacingStructure = false;
                //Destroy(dummyPlacingObject);
            }
        }
    }

    private void OnPlayerAttemptBuild()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Create a new BuildingRequest entity
        Entity requestEntity = ecb.CreateEntity();
        ecb.Instantiate(structureEntityPrefab);
        /*
        ecb.AddComponent(requestEntity, new BuildingRequestData
        {
            StructurePrefab = structureEntityPrefab,
            Size = structureSize
        });
        */
        isPlacingStructure = true;
        ecb.Playback(entityManager);
        ecb.Dispose();
        Debug.Log("OnPlayerAttemptBuild");
    }
}

