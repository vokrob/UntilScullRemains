using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("Префабы комнат")]
    [Tooltip("Начальная комната — всегда спавнится в центре первой")]
    [SerializeField] private GameObject startingRoomPrefab;
    [Tooltip("Префабы остальных комнат — выбираются случайно")]
    [SerializeField] private List<GameObject> roomPrefabs = new List<GameObject>();

    [Header("Настройки генерации")]
    [SerializeField] private int maxRooms = 15;
    [SerializeField] private int minRooms = 10;
    [SerializeField] private RoomCamera roomCamera;

    int roomWidth = 29;
    int roomHeight = 17;

    int gridSizeX = 10;
    int gridSizeY = 10;

    private List<GameObject> roomObjects = new List<GameObject>();

    private Queue<Vector2Int> roomQueue = new Queue<Vector2Int>();

    private int[,] roomGrid;

    private int roomCount;

    private bool generationComplete = false;

    private Room currentRoom;

    private void Start()
    {
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue = new Queue<Vector2Int>();

        Vector2Int inititalRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(inititalRoomIndex);

        MoveCameraToRoom(inititalRoomIndex);

        NotifyPlayerEnteredRoom(inititalRoomIndex);
    }
	
    public void NotifyPlayerEnteredRoom(Vector2Int roomIndex)
    {
        Room room = GetRoomScriptAt(roomIndex);
        if (room == null) return;

        currentRoom = room;
        room.OnPlayerEnter();
    }

    private void Update()
    {
        if (roomQueue.Count > 0 && roomCount < maxRooms && !generationComplete)
        {
            Vector2Int roomIndex = roomQueue.Dequeue();
            int gridX = roomIndex.x;
            int gridY = roomIndex.y;

            TryGenerateRoom(new Vector2Int(gridX - 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX + 1, gridY));
            TryGenerateRoom(new Vector2Int(gridX, gridY - 1));
            TryGenerateRoom(new Vector2Int(gridX, gridY + 1));
        }
        else if(roomCount < minRooms)
        {
            RegenerateRooms();
        }
        else if (!generationComplete)
        {
            generationComplete = true;
        }
    }
    private void StartRoomGenerationFromRoom(Vector2Int roomIndex)
    {
        roomQueue.Enqueue(roomIndex);
        int x = roomIndex.x;
        int y = roomIndex.y;
        roomGrid[x, y] = 1;
        roomCount++;
        var initialRoom = Instantiate(startingRoomPrefab, GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        initialRoom.name = $"Room-{roomCount} (Start)";
        initialRoom.GetComponent<Room>().RoomIndex = roomIndex;
        roomObjects.Add(initialRoom);
    }

    private GameObject GetRandomRoomPrefab()
    {
        if (roomPrefabs == null || roomPrefabs.Count == 0)
        {
            return startingRoomPrefab;
        }

        return roomPrefabs[Random.Range(0, roomPrefabs.Count)];
    }

    private bool TryGenerateRoom(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;

        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
            return false;

        if (roomGrid[x, y] != 0)
            return false;

        if (roomCount >= maxRooms)
            return false;

        if (Random.value < 0.5f && roomIndex != Vector2Int.zero)
            return false;

        if (CountAdjacentRooms(roomIndex) > 1)
            return false;

        roomQueue.Enqueue(roomIndex);
        roomGrid[x, y] = 1;
        roomCount++;

        var newRoom = Instantiate(GetRandomRoomPrefab(), GetPositionFromGridIndex(roomIndex), Quaternion.identity);
        newRoom.GetComponent<Room>().RoomIndex = roomIndex;
        newRoom.name = $"Room-{roomCount}";
        roomObjects.Add(newRoom);

        OpenDoors(newRoom, x, y);

        return true;
    }
	
    private void RegenerateRooms()
    {
        roomObjects.ForEach(Destroy);
        roomObjects.Clear();
        roomGrid = new int[gridSizeX, gridSizeY];
        roomQueue.Clear();
        roomCount = 0;
        generationComplete = false;

        Vector2Int initialRoomIndex = new Vector2Int(gridSizeX / 2, gridSizeY / 2);
        StartRoomGenerationFromRoom(initialRoomIndex);
    }

    void OpenDoors(GameObject room, int x, int y)
    {
        Room newRoomScript = room.GetComponent<Room>();

        Room leftRoomScripts = GetRoomScriptAt(new Vector2Int(x - 1, y));
        Room rightRoomScripts = GetRoomScriptAt(new Vector2Int(x + 1, y));
        Room topRoomScripts = GetRoomScriptAt(new Vector2Int(x, y + 1));
        Room bottomRoomScripts = GetRoomScriptAt(new Vector2Int(x, y - 1));

        // Determine which doors to open based on the direction
        if (x > 0 && roomGrid[x - 1, y] != 0)
        {
            // Neighbour room to the left
            newRoomScript.OpenDoor(Vector2Int.left);
            if (leftRoomScripts != null) leftRoomScripts.OpenDoor(Vector2Int.right);

            SetupDoorTrigger(newRoomScript.leftDoor, new Vector2Int(x - 1, y), Vector2Int.right);
            SetupDoorTrigger(leftRoomScripts.rightDoor, new Vector2Int(x, y), Vector2Int.left);
        }
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0)
        {
            // Neighbour room to the right
            newRoomScript.OpenDoor(Vector2Int.right);
            if (rightRoomScripts != null) rightRoomScripts.OpenDoor(Vector2Int.left);

            SetupDoorTrigger(newRoomScript.rightDoor, new Vector2Int(x + 1, y), Vector2Int.left);
            SetupDoorTrigger(rightRoomScripts.leftDoor, new Vector2Int(x, y), Vector2Int.right);
        }
        if (y > 0 && roomGrid[x, y - 1] != 0)
        {
            // Neighbour room below
            newRoomScript.OpenDoor(Vector2Int.down);
            if (bottomRoomScripts != null) bottomRoomScripts.OpenDoor(Vector2Int.up);

            SetupDoorTrigger(newRoomScript.bottomDoor, new Vector2Int(x, y - 1), Vector2Int.up);
            SetupDoorTrigger(bottomRoomScripts.topDoor, new Vector2Int(x, y), Vector2Int.down);
        }
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0)
        {
            // Neighbour room above
            newRoomScript.OpenDoor(Vector2Int.up);
            if (topRoomScripts != null) topRoomScripts.OpenDoor(Vector2Int.down);

            SetupDoorTrigger(newRoomScript.topDoor, new Vector2Int(x, y + 1), Vector2Int.down);
            SetupDoorTrigger(topRoomScripts.bottomDoor, new Vector2Int(x, y), Vector2Int.up);
        }
    }

    Room GetRoomScriptAt(Vector2Int index)
    {
        GameObject roomObject = roomObjects.Find(r => r.GetComponent<Room>().RoomIndex == index);
        if (roomObject != null)
            return roomObject.GetComponent<Room>();
        return null;
    }
    private int CountAdjacentRooms(Vector2Int roomIndex)
    {
        int x = roomIndex.x;
        int y = roomIndex.y;
        int count = 0;

        if (x > 0 && roomGrid[x - 1, y] != 0) count++; // Left neighbour
        if (x < gridSizeX - 1 && roomGrid[x + 1, y] != 0) count++; // Right neigbour
        if (y > 0 && roomGrid[x, y - 1] != 0) count++; // Bottom neigbour
        if (y < gridSizeY - 1 && roomGrid[x, y + 1] != 0) count++; // Top Neigbour

        return count;
    }

    private Vector3 GetPositionFromGridIndex(Vector2Int gridIndex)
    {
        int gridX = gridIndex.x;
        int gridY = gridIndex.y;
        return new Vector3(roomWidth * (gridX - gridSizeX / 2),
            roomHeight * (gridY - gridSizeY / 2));
    }

    private void OnDrawGizmos()
    {
        Color gizmosColor = new Color(0, 1, 1, 0.05f);
        Gizmos.color = gizmosColor;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 position = GetPositionFromGridIndex(new Vector2Int(x, y));
                Gizmos.DrawWireCube(position, new Vector3(roomWidth, roomHeight, 1));
            }
        }
    }

    public Vector3 GetDoorSpawnPosition(Vector2Int roomIndex, Vector2Int spawnSide)
    {
        Vector3 roomCenter = GetPositionFromGridIndex(roomIndex);
        float offsetX = 0f;
        float offsetY = 0f;

        float safeDistance = 3.0f;

        if (spawnSide == Vector2Int.up)
        {
            offsetY = (roomHeight / 2) - safeDistance;
        }
        else if (spawnSide == Vector2Int.down)
        {
            offsetY = -(roomHeight / 2) + safeDistance;
        }
        else if (spawnSide == Vector2Int.right)
        {
            offsetX = (roomWidth / 2) - safeDistance;
        }
        else if (spawnSide == Vector2Int.left)
        {
            offsetX = -(roomWidth / 2) + safeDistance;
        }

        return new Vector3(roomCenter.x + offsetX, roomCenter.y + offsetY, 0f);
    }

    private void SetupDoorTrigger(GameObject doorObject, Vector2Int targetRoom, Vector2Int spawnSide)
    {
        if (doorObject != null)
        {
            DoorTrigger trigger = doorObject.GetComponent<DoorTrigger>();
            if (trigger != null)
            {
                trigger.targetRoomIndex = targetRoom;
                trigger.spawnSide = spawnSide;
            }
        }
    }

    public void MoveCameraToRoom(Vector2Int roomIndex)
    {
        if (roomCamera != null)
        {
            Vector3 roomCenter = GetPositionFromGridIndex(roomIndex);
            roomCamera.SetTargetPosition(roomCenter);
        }
    }
}