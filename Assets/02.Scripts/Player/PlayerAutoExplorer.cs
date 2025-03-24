using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoExplorer : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Player player = new();

    private Stack<Vector2Int> dfsStack = new();
    private HashSet<Vector2Int> visited = new();
    private HashSet<Vector2Int> visitedRoom = new();
    private Stack<Vector2Int> cameFrom = new();
    private bool isMoving = false;
    private bool isFighting = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => mapGenerator.GetAllRooms().Count > 0);

        player.Init();

        Vector2Int start = mapGenerator.GetAllRooms()[0].pos;
        transform.position = new Vector3(start.x, 0.5f, start.y);
        dfsStack.Push(start);
        visited.Clear();
        visitedRoom.Clear();
        cameFrom.Clear();
    }

    private void Update()
    {
        if (isMoving || isFighting) return;
        if (dfsStack.Count == 0) return;


        Vector2Int current = dfsStack.Peek();
        if (!visited.Contains(current)) visited.Add(current);

        List<Vector2Int> neighbors = GetNeighbors(current);
        bool foundUnvisited = false;

        foreach (var neighbor in neighbors)
        {
            if (!visited.Contains(neighbor) && !dfsStack.Contains(neighbor))
            {
                dfsStack.Push(neighbor);
                cameFrom.Push(current);
                foundUnvisited = true;
                break; // DFS: 하나만 먼저 처리
            }
        }

        if (foundUnvisited)
        {
            StartCoroutine(MoveToRoom(dfsStack.Peek()));
        }
        else
        {
            dfsStack.Pop();

            if (IsAllVisited())
            {
                Debug.Log("모든 방 탐험 완료!");
                // GameManager.Instance.OnFloorCleared?.Invoke();
            }
            else if (cameFrom.Count > 0)
            {
                StartCoroutine(MoveToRoom(cameFrom.Pop()));
            }
        }
    }
    bool IsAllVisited()
    {
        return visitedRoom.Count >= mapGenerator.GetAllRooms().Count;
    }

    IEnumerator MoveToRoom(Vector2Int destination)
    {
        isMoving = true;
        Vector3 target = new Vector3(destination.x, 0.5f, destination.y);

        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, player.moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        yield return new WaitForSeconds(0.2f);

        TryCombat(destination);
    }

    private void TryCombat(Vector2Int roomPos)
    {
        // 방인 경우만 전투, 전투는 1회만 발생
        if (!mapGenerator.roomDataMap.TryGetValue(roomPos, out RoomData room))
            return;

        if (visitedRoom.Contains(roomPos))
            return;

        visitedRoom.Add(roomPos);

        if (room.enemies.Count > 0)
        {
            StartCoroutine(CombatCoroutine(room));
        }
    }

    private IEnumerator CombatCoroutine(RoomData room)
    {
        isFighting = true;
        Debug.Log($"전투 시작: {room.enemies.Count}명의 적");
        yield return new WaitForSeconds(1f); // 전투 연출 시간
        room.enemies.Clear();
        Debug.Log("전투 완료");
        isFighting = false;
    }

    List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> results = new();
        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir;
            if (mapGenerator.floorPositions.Contains(neighbor))
            {
                results.Add(neighbor);
            }
        }

        return results;
    }
}
