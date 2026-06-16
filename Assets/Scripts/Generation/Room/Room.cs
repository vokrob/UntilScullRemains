using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    [Header("Открытые двери")]
    [SerializeField] public GameObject topDoor;
    [SerializeField] public GameObject bottomDoor;
    [SerializeField] public GameObject leftDoor;
    [SerializeField] public GameObject rightDoor;

    [Header("Закрытые двери")]
    [SerializeField] private GameObject closedTopDoor;
    [SerializeField] private GameObject closedBottomDoor;
    [SerializeField] private GameObject closedLeftDoor;
    [SerializeField] private GameObject closedRightDoor;

    public Vector2Int RoomIndex { get; set; }

    private readonly HashSet<Vector2Int> connectedDirections = new HashSet<Vector2Int>();

    private readonly List<EnemyEntity> slimeEnemies = new List<EnemyEntity>();
    private readonly List<MushroomEntity> mushroomEnemies = new List<MushroomEntity>();
    private int aliveEnemies;

    private readonly List<Behaviour> enemyAIs = new List<Behaviour>();

    public bool IsCleared { get; private set; }
    public bool HasLivingEnemies => aliveEnemies > 0;

    private void Awake()
    {
        ResolveClosedDoors();
        RegisterEnemies();
    }
    
    public void OpenDoor(Vector2Int direction)
    {
        connectedDirections.Add(direction);
        SetDoorState(direction, opened: true);
    }

    public void LockDoors()
    {
        foreach (Vector2Int dir in connectedDirections)
        {
            SetDoorState(dir, opened: false);
        }
    }

    public void UnlockDoors()
    {
        foreach (Vector2Int dir in connectedDirections)
        {
            SetDoorState(dir, opened: true);
        }
    }

    private void SetDoorState(Vector2Int direction, bool opened)
    {
        GameObject openedDoor = GetOpenedDoor(direction);
        GameObject closedDoor = GetClosedDoor(direction);

        if (openedDoor != null) openedDoor.SetActive(opened);
        if (closedDoor != null) closedDoor.SetActive(!opened);
    }

    private GameObject GetOpenedDoor(Vector2Int d)
    {
        if (d == Vector2Int.up) return topDoor;
        if (d == Vector2Int.down) return bottomDoor;
        if (d == Vector2Int.left) return leftDoor;
        if (d == Vector2Int.right) return rightDoor;
        return null;
    }

    private GameObject GetClosedDoor(Vector2Int d)
    {
        if (d == Vector2Int.up) return closedTopDoor;
        if (d == Vector2Int.down) return closedBottomDoor;
        if (d == Vector2Int.left) return closedLeftDoor;
        if (d == Vector2Int.right) return closedRightDoor;
        return null;
    }

    private void ResolveClosedDoors()
    {
        if (closedTopDoor == null) closedTopDoor = FindClosedSibling(topDoor);
        if (closedBottomDoor == null) closedBottomDoor = FindClosedSibling(bottomDoor);
        if (closedLeftDoor == null) closedLeftDoor = FindClosedSibling(leftDoor);
        if (closedRightDoor == null) closedRightDoor = FindClosedSibling(rightDoor);
    }

    private GameObject FindClosedSibling(GameObject openedDoor)
    {
        if (openedDoor == null || openedDoor.transform.parent == null) return null;

        string closedName = openedDoor.name.Replace("Opened", "Closed");
        Transform sibling = openedDoor.transform.parent.Find(closedName);

        if (sibling == null)
        {
            return null;
        }
        return sibling.gameObject;
    }

    private void RegisterEnemies()
    {
        slimeEnemies.Clear();
        mushroomEnemies.Clear();

        slimeEnemies.AddRange(GetComponentsInChildren<EnemyEntity>(true));
        mushroomEnemies.AddRange(GetComponentsInChildren<MushroomEntity>(true));

        aliveEnemies = slimeEnemies.Count + mushroomEnemies.Count;
        IsCleared = aliveEnemies == 0;

        foreach (EnemyEntity e in slimeEnemies) e.OnDeath += Enemy_OnDeath;
        foreach (MushroomEntity e in mushroomEnemies) e.OnDeath += Enemy_OnDeath;

        enemyAIs.Clear();
        foreach (EnemyAI ai in GetComponentsInChildren<EnemyAI>(true)) { ai.enabled = false; enemyAIs.Add(ai); }
        foreach (BatAI ai in GetComponentsInChildren<BatAI>(true)) { ai.enabled = false; enemyAIs.Add(ai); }
        foreach (MushroomAI ai in GetComponentsInChildren<MushroomAI>(true)) { ai.enabled = false; enemyAIs.Add(ai); }
    }

    private void ActivateEnemies()
    {
        foreach (Behaviour ai in enemyAIs)
            if (ai != null) ai.enabled = true;
    }

    private void Enemy_OnDeath(object sender, EventArgs e)
    {
        aliveEnemies = Mathf.Max(0, aliveEnemies - 1);

        if (aliveEnemies == 0)
        {
            IsCleared = true;
            UnlockDoors();
        }
    }

    public void OnPlayerEnter()
    {
        ActivateEnemies();

        if (HasLivingEnemies)
        {
            LockDoors();
        }
        else
        {
            UnlockDoors();
        }
    }

    private void OnDestroy()
    {
        foreach (EnemyEntity e in slimeEnemies)
            if (e != null) e.OnDeath -= Enemy_OnDeath;

        foreach (MushroomEntity e in mushroomEnemies)
            if (e != null) e.OnDeath -= Enemy_OnDeath;
    }
}
