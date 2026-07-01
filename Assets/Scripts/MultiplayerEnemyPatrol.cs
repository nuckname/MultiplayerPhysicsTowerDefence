using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

// Requires a NetworkTransform to automatically sync position and rotation to clients
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(NetworkObject))]
public class MultiplayerEnemyPatrol : NetworkBehaviour
{
    [Header("Path Settings")]
    [Tooltip("Assign empty GameObjects here to act as the path points.")]
    public Transform[] waypoints;
    public float moveSpeed = 5f;
    public float waypointTolerance = 0.5f; // How close to get before switching to the next point
    public float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;

    // This method fires exactly when the object spawns on the network
    public override void OnNetworkSpawn()
    {
        // If this instance is running on a client, disable this script.
        // We only want the Server/Host to calculate movement to prevent desync.
        if (!IsServer)
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        // Safety check to ensure only the server runs this logic
        // Also ensures we actually have a path to follow
        if (!IsServer || waypoints.Length == 0) return;

        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        // Get our current destination
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;

        // Move the enemy towards the waypoint
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;

        // Rotate the enemy to face the waypoint smoothly
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        // Check if we are close enough to the waypoint to target the next one
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointTolerance)
        {
            // Loop back to the start of the array if we hit the end
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}