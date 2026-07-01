using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("The enemy prefab to spawn. MUST have a NetworkObject component.")]
    public GameObject enemyPrefab;
    
    [Tooltip("Where the enemy should appear. If empty, uses the Spawner's position.")]
    public Transform spawnPoint;
    
    [Tooltip("How often to spawn an enemy (in seconds).")]
    public float spawnInterval = 5f;

    private float timer = 0f;
    public bool canSpawnEnemies = false;

    // --- NEW VARIABLES FOR ROUND LOGIC ---
    private int enemiesToSpawnThisRound = 0;
    private int enemiesSpawnedSoFar = 0;

    // Helper property so the FSM knows when the spawner is done
    public bool HasFinishedSpawning => enemiesSpawnedSoFar >= enemiesToSpawnThisRound;

    // Called by the FSM to kick off a wave
    public void StartSpawningForRound(int amountToSpawn)
    {
        enemiesToSpawnThisRound = amountToSpawn;
        enemiesSpawnedSoFar = 0;
        timer = 0f; // Reset timer so the first spawn happens predictably
        canSpawnEnemies = true;
    }

    void Update()
    {
        // Safety check: ONLY the server is allowed to spawn networked objects.
        // If a client runs this, we just ignore it.
        if (!IsServer) return;
        if (!canSpawnEnemies) return;
        
        // Simple timer to spawn enemies continuously
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnNetworkedEnemy();
            timer = 0f; // Reset the timer
        }
    }

    private void SpawnNetworkedEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy Prefab is not assigned in the Spawner!");
            return;
        }

        // Determine where to spawn. 
        Vector3 position = spawnPoint.position;

        // Instantiate the object locally on the server
        GameObject spawnedEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);

        // Grab the NetworkObject and broadcast the spawn to all clients
        NetworkObject netObj = spawnedEnemy.GetComponent<NetworkObject>();
        netObj.Spawn(true); 

        // --- NEW LOGIC: Track spawns and tell the State Manager ---
        enemiesSpawnedSoFar++;
        RoundStateManager.Instance.activeEnemyCount++;

        // Stop the spawner if we reached the limit for this round
        if (enemiesSpawnedSoFar >= enemiesToSpawnThisRound)
        {
            canSpawnEnemies = false;
        }
    }
}