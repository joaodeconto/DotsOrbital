using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private Button resetScene;
    [SerializeField] private Button togglePhysics;

    void Start()
    {
        resetScene.onClick.AddListener(() => ResetScene());
        togglePhysics.onClick.AddListener(() => TogglePhysics() );
    }


    private void TogglePhysics()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        Entity ent = entityManager.CreateEntityQuery(typeof(GameOptions)).GetSingletonEntity();

        var toggleData = entityManager.GetComponentData<GameOptions>(ent);
        toggleData.OnPhysicsToggle = true;
        entityManager.SetComponentData(ent, toggleData);
    }


    private void ResetScene()
    {
        SceneManager.LoadScene(0);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}