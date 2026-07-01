using UnityEngine;

public class RoundInProgressState : RoundBaseState
{
    public override void EnterState(RoundStateManager stateManager)
    {
        Debug.Log($"Round {stateManager.currentRound.Value} Started!");

        if (stateManager.IsServer)
        {
            // Configure how many enemies spawn this round. 
            int amountToSpawn = stateManager.currentRound.Value * 5; 
            
            if (stateManager.Spawner != null)
            {
                stateManager.Spawner.StartSpawningForRound(amountToSpawn);
            }
        }
    }

    public override void UpdateState(RoundStateManager stateManager)
    {
        if (!stateManager.IsServer) return; 

        // WIN CONDITION: Spawner is empty AND all enemies on the map are dead
        if (stateManager.Spawner.HasFinishedSpawning && stateManager.activeEnemyCount <= 0)
        {
            Debug.Log("All enemies defeated! Wave cleared.");
            stateManager.ServerSwitchState(RoundPhase.Over);
        }
    }

    public override void ExitState(RoundStateManager stateManager)
    {
        if (stateManager.IsServer)
        {
            if (stateManager.Spawner != null)
            {
                stateManager.Spawner.canSpawnEnemies = false;
            }
        }
    }

    public override void OnCollisionEnter2D(RoundStateManager stateManager, Collision2D other)
    {
        // Base damage logic goes here
    }
}