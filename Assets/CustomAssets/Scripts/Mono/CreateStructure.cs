using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

public static class TileOccupancy
{
    public static Dictionary<Vector2Int, bool> OccupancyMap = new Dictionary<Vector2Int, bool>();
}

public class CreateStructure : MonoBehaviour
{
    [SerializeField] private int2 gridPosition;
    [SerializeField] private int2 gridSize;
    [SerializeField] private Button structureBtn;
    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private float hexRadius = 4.4f;


    private bool isPlacingStructure;
    private EntityManager entityManager;
    private Entity structureEntityPrefab;
    private GameObject dummyPlacingObject;
    private float hexWidth;
    private float hexHeight;

    private void Awake()
    {
        hexWidth = hexRadius * 2f;
        hexHeight = Mathf.Sqrt(3f) * hexRadius;
    }

    private void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Retrieve the EntitiesReferences singleton
        var entitiesReferencesQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        var entitiesReferences = entitiesReferencesQuery.GetSingleton<EntitiesReferences>();

        structureEntityPrefab = entitiesReferences.structureTestEntity;

        //structureBtn.onClick.AddListener(() => OnPlayerAttemptBuild(structureEntityPrefab, gridPosition, gridSize));
        structureBtn.onClick.AddListener(() => InstantiateDummyStructure());
    }

    private void InstantiateDummyStructure()
    {
        dummyPlacingObject = Instantiate(dummyPrefab); 
        isPlacingStructure = true;
    }

    private void Update()
    {
        if (isPlacingStructure)
        {
            // Cast a ray from the camera through the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Assuming the ground is at y = 0

            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                // Get the point where the mouse ray intersects the ground plane
                Vector3 worldPosition = ray.GetPoint(rayDistance);

                // Convert world position to hex grid coordinates
                Vector2Int hexCoords = WorldToHex(worldPosition);

                // Get the center position of the hex tile
                Vector3 tileCenterPosition = HexToWorld(hexCoords);

                // Update the position of the dummy object
                dummyPlacingObject.transform.position = tileCenterPosition;

                if (IsTileOccupied(hexCoords))
                {
                    Debug.Log("HexOccupied " +  hexCoords);
                }
            }

           

            if (Input.GetMouseButtonDown(0))
            {
                isPlacingStructure = false;
                Destroy(dummyPlacingObject);

                // Instantiate the actual structure at the tile position
                // Instantiate(structurePrefab, dummyPlacingObject.transform.position, Quaternion.identity);
            }
        }
    }

    private bool IsTileOccupied(Vector2Int hexCoords)
    {
        // Create a query for tiles with matching coordinates
        var tileQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<HexTileData>(),
            ComponentType.ReadOnly<LocalToWorld>()
        );

        using (var tiles = tileQuery.ToEntityArray(Allocator.TempJob))
        {
            foreach (var tileEntity in tiles)
            {
                var tileData = entityManager.GetComponentData<HexTileData>(tileEntity);
                if (tileData.tileCoordinates.Equals(hexCoords))
                {
                    return tileData.isOccupied;
                }
            }
        }
        return false;
    }

    private Vector2Int WorldToHex(Vector3 position)
    {
        float q = (Mathf.Sqrt(3f) / 3f * position.x - 1f / 3f * position.z) / hexRadius;
        float r = (2f / 3f * position.z) / hexRadius;

        // Round to the nearest hex tile
        int roundedQ, roundedR;
        HexRound(q, r, out roundedQ, out roundedR);

        return new Vector2Int(roundedQ, roundedR);
    }

    private void HexRound(float q, float r, out int roundedQ, out int roundedR)
    {
        float x = q;
        float z = r;
        float y = -x - z;

        int rx = Mathf.RoundToInt(x);
        int ry = Mathf.RoundToInt(y);
        int rz = Mathf.RoundToInt(z);

        float x_diff = Mathf.Abs(rx - x);
        float y_diff = Mathf.Abs(ry - y);
        float z_diff = Mathf.Abs(rz - z);

        if (x_diff > y_diff && x_diff > z_diff)
        {
            rx = -ry - rz;
        }
        else if (y_diff > z_diff)
        {
            ry = -rx - rz;
        }
        else
        {
            rz = -rx - ry;
        }

        roundedQ = rx;
        roundedR = rz;
    }

    private Vector3 HexToWorld(Vector2Int hexCoords)
    {
        int q = hexCoords.x;
        int r = hexCoords.y;

        float x = hexRadius * Mathf.Sqrt(3f) * (q + r / 2f);
        float z = hexRadius * 1.5f * r;

        return new Vector3(x, 0f, z); // Assuming y = 0 for ground level
    }


void OnPlayerAttemptBuild(Entity structurePrefab, int2 gridPosition, int2 size)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        // Create a new BuildingRequest entity
        Entity requestEntity = ecb.CreateEntity();
        ecb.AddComponent(requestEntity, new BuildingRequestData
        {
            StructurePrefab = structurePrefab,
            GridPosition = gridPosition,
            Size = size
        });

        ecb.Playback(entityManager);
        ecb.Dispose();
        Debug.Log("OnPlayerAttemptBuild");
    }
}
