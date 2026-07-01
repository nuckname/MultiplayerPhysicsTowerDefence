using UnityEngine;

public class RoundInProgressState : RoundBaseState
{
    public override void EnterState(RoundStateManager stateManager)
    {
        Debug.Log("Round Started!");
        
        // CLIENT & SERVER: Update UI to show "Wave Started"
        // UIManager.Instance.ShowWaveStarted();

        if (stateManager.IsServer)
        {
            // SERVER ONLY: Start the enemy spawners
            Debug.Log("Server is starting the spawners.");
            stateManager.enemySpawner.canSpawnEnemies = true;
        }
    }

    public override void UpdateState(RoundStateManager stateManager)
    {
        if (!stateManager.IsServer) return; 

        // SERVER ONLY: Check win/loss conditions every frame
        // Example: If base health <= 0, tell everyone the round is over
        // if (baseHealth <= 0) 
        // {
        //     stateManager.ServerSwitchState(RoundPhase.Over);
        // }
    }

    public override void ExitState(RoundStateManager stateManager)
    {
        Debug.Log("Round Finished. Cleaning up active round elements.");
        // Logic to disable spawners, stop round timer, etc.
    }

    public override void OnCollisionEnter2D(RoundStateManager stateManager, Collision2D other)
    {
        if (!stateManager.IsServer) return;

        // SERVER ONLY: Detect if an enemy hits the base during the round
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Base took damage!");
            // Calculate damage here
        }
    }
}