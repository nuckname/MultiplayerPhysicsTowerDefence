using UnityEngine;

public abstract class RoundBaseState
{
    // Called once the moment this state becomes the active state
    public abstract void EnterState(RoundStateManager stateManager);
    
    // Called every frame by the State Manager's Update loop
    public abstract void UpdateState(RoundStateManager stateManager);
    
    // Called once right before switching to a different state (Great for cleanup)
    public abstract void ExitState(RoundStateManager stateManager);
    
    // Handles physics collisions depending on the current state
    public abstract void OnCollisionEnter2D(RoundStateManager stateManager, Collision2D other);
}