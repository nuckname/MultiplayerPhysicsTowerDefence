using UnityEngine;
using Unity.Netcode;

public class RoundStateManager : NetworkBehaviour
{
    // Instantiate all possible states here so they stay in memory and don't need recreating
    public RoundInProgressState roundInProgressState = new RoundInProgressState();
    public RoundOverState roundOverState = new RoundOverState();

    public EnemySpawner enemySpawner;
    
    // The NetworkVariable holds the current state Enum. 
    // Everyone can read it, but ONLY the server can change it.
    public NetworkVariable<RoundPhase> currentNetworkPhase = new NetworkVariable<RoundPhase>(
        RoundPhase.InProgress,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    // Tracks the current active state locally for the FSM logic
    private RoundBaseState currentState;

    public override void OnNetworkSpawn()
    {
        // Subscribe to network state changes so clients know when the server switches states
        currentNetworkPhase.OnValueChanged += OnNetworkPhaseChanged;

        // Initialize the local state based on whatever the server says the current phase is upon spawning
        TransitionLocalState(currentNetworkPhase.Value);
    }

    public override void OnNetworkDespawn()
    {
        // Always clean up subscriptions to prevent memory leaks
        currentNetworkPhase.OnValueChanged -= OnNetworkPhaseChanged;
    }

    void Update()
    {
        // Delegate the Update loop to the current state
        if (currentState != null)
        {
            currentState.UpdateState(this);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // Delegate collision events to the current state
        if (currentState != null)
        {
            currentState.OnCollisionEnter2D(this, other);
        }
    }

    // SERVER ONLY: Call this to change the game state for everyone.
    public void ServerSwitchState(RoundPhase newPhase)
    {
        if (!IsServer) return; // Safety check
        
        // Changing this variable will automatically trigger OnNetworkPhaseChanged for all clients
        currentNetworkPhase.Value = newPhase; 
    }

    // Triggered automatically on all machines when currentNetworkPhase.Value changes
    private void OnNetworkPhaseChanged(RoundPhase oldPhase, RoundPhase newPhase)
    {
        TransitionLocalState(newPhase);
    }

    // Handles the actual FSM transition locally on every machine
    private void TransitionLocalState(RoundPhase newPhase)
    {
        // 1. Run cleanup logic for the old state (if one exists)
        if (currentState != null)
        {
            currentState.ExitState(this);
        }

        // 2. Determine which state class matches the new enum
        switch (newPhase)
        {
            case RoundPhase.InProgress:
                currentState = roundInProgressState;
                break;
            case RoundPhase.Over:
                currentState = roundOverState;
                break;
        }

        // 3. Run setup logic for the new state
        currentState.EnterState(this);
    }
}