using UnityEngine;
using UnityEngine.InputSystem;

public class RoundOverState : RoundBaseState
{
    public override void EnterState(RoundStateManager stateManager)
    {
        Debug.Log($"Round Over State!");
        
        // Check if they beat the final round
        if (stateManager.currentRound.Value >= stateManager.maxRounds)
        {
            Debug.Log("Game Complete! You beat all rounds.");
            // Trigger game win UI
        }
        else
        {
            Debug.Log("Round Over! Press Space to start next wave.");
            // Trigger between-round UI
        }
    }

    public override void UpdateState(RoundStateManager stateManager)
    {
        if (!stateManager.IsServer) return;

        // If not max round, wait for input to progress
        if (stateManager.currentRound.Value < stateManager.maxRounds)
        {
            // Example: Press Space to start the next round
            if (Keyboard.current.spaceKey.wasPressedThisFrame) 
            { 
                stateManager.currentRound.Value++; // Increment round network variable
                stateManager.ServerSwitchState(RoundPhase.InProgress); 
            }
        }
    }

    public override void ExitState(RoundStateManager stateManager)
    {
        // Hide UI
    }

    public override void OnCollisionEnter2D(RoundStateManager stateManager, Collision2D other) { }
}