using UnityEngine;

public class RoundOverState : RoundBaseState
{
    public override void EnterState(RoundStateManager stateManager)
    {
        Debug.Log("Round Over!");
        
        // CLIENT & SERVER: Show victory/defeat UI, tally score
        // UIManager.Instance.ShowGameOverScreen();

        if (stateManager.IsServer)
        {
            stateManager.enemySpawner.canSpawnEnemies = false;
        }
    }

    public override void UpdateState(RoundStateManager stateManager)
    {
        if (!stateManager.IsServer) return;

        // SERVER ONLY: Wait for server host to press a button to restart
        // if (Input.GetKeyDown(KeyCode.Space)) 
        // { 
        //     stateManager.ServerSwitchState(RoundPhase.InProgress); 
        // }
    }

    public override void ExitState(RoundStateManager stateManager)
    {
        Debug.Log("Hiding summary screen, preparing for new round.");
        // CLIENT & SERVER: Hide UI menus
    }

    public override void OnCollisionEnter2D(RoundStateManager stateManager, Collision2D other)
    {
        // Ignore collisions while the round is over
    }
}