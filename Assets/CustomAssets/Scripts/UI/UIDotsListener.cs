using UnityEngine;
using TMPro;
using Unity.Entities;
using Unity.Collections;

public class UIDotsListener : MonoBehaviour
{
    [SerializeField] private TMP_Text spawnedCountText;

    private EntityManager entityManager;

    private void Start()
    {
        if (spawnedCountText == null)
        {
            Debug.LogError("TMP_Text reference is missing!");
            return;
        }

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    private void Update()
    {
        // Query for UIUpdateEvent entities
        var query = entityManager.CreateEntityQuery(typeof(AstroBodySpawner));
        if (query.IsEmpty) return;

        // Get the first UIUpdateEvent
        var events = query.ToComponentDataArray<AstroBodySpawner>(Allocator.Temp);
        int count = 0;
        for (int i = 0; i < events.Length; i++)
        {
            count += events[i].spawnedCount;
        }
        spawnedCountText.text = $"Spawned Count: {count}";
        events.Dispose();
    }
}
