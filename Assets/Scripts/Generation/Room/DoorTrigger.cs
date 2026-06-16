using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public Vector2Int targetRoomIndex;
    public Vector2Int spawnSide;

    private bool isTransitioning = false;
    private Transform playerInRange;

    private void Update()
    {
        if (playerInRange != null && !isTransitioning && Input.GetKeyDown(KeyCode.E))
        {
            UseDoor(playerInRange);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other is CapsuleCollider2D && other.CompareTag("Player"))
        {
            playerInRange = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other is CapsuleCollider2D && other.CompareTag("Player"))
        {
            playerInRange = null;
        }
    }

    private void UseDoor(Transform player)
    {
        RoomManager roomManager = FindAnyObjectByType<RoomManager>();
        if (roomManager == null) return;

        isTransitioning = true;

        Vector3 spawnPosition = roomManager.GetDoorSpawnPosition(targetRoomIndex, spawnSide);
        player.position = spawnPosition;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        roomManager.MoveCameraToRoom(targetRoomIndex);

        roomManager.NotifyPlayerEnteredRoom(targetRoomIndex);

        playerInRange = null;
        Invoke(nameof(ResetTransition), 0.5f);
    }

    private void ResetTransition()
    {
        isTransitioning = false;
    }
}
