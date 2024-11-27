using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (RefRW<GameOptions> gameOptions in SystemAPI.Query<RefRW<GameOptions>>())
        {
            gameOptions.ValueRW.OnPhysicsToggle = false;
        }

        /*
        foreach (RefRW<CannonShoot> cannonShoot in SystemAPI.Query<RefRW<CannonShoot>>())
        {
            cannonShoot.ValueRW.OnShootCannonBall = false;
        }
        foreach (RefRW<Selected> selected in SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>()) 
        {
            selected.ValueRW.OnSelected = false;
            selected.ValueRW.OnDeselected = false;
        }

        foreach (RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.onHealthChanged = false;
        }bodySpawner


        foreach (RefRW<ShootAttack> shootAttack in SystemAPI.Query<RefRW<ShootAttack>>())
        {
            shootAttack.ValueRW.OnShoot.isTriggered = false;

        }

        foreach (RefRW<AstroBodySpawner> bodySpawner in SystemAPI.Query<RefRW<AstroBodySpawner>>())
        {
            bodySpawner.ValueRW.onSpawned = false;

        }
        
        */

    }
}
