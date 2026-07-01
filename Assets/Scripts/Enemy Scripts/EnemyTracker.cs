using UnityEngine;
using Unity.Netcode;

public class EnemyTracker : NetworkBehaviour
{
    // This is a built-in Netcode method that fires right before the object is destroyed/removed
    public override void OnNetworkDespawn()
    {
        // Only the server maintains the enemy count
        if (IsServer && RoundStateManager.Instance != null)
        {
            RoundStateManager.Instance.activeEnemyCount--;
        }
    }
}