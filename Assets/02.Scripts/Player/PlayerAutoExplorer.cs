using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.RestService;
using UnityEngine;

public class PlayerAutoExplorer : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Player player = new();

    private Stack<Vector2Int> dfsStack = new();
    private HashSet<Vector2Int> visited = new();
    private Vector3 targetPos;
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
    }

    private void Update()
    {
        if (isMoving || isFighting) return;

        if(dfsStack.Count > 0)
        {
            Vector2Int current = dfsStack.Pop();
            if (!visited.Contains(current))
            {
                visited.Add(current);
                StartCoroutine(MoveToRoom(current));

                foreach (var neighbor in GetNeighbors(current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        dfsStack.Push(neighbor);
                    }
                }
            }
        }
        else
        {
            Debug.Log("모든 방 탐험 완료! 층 클리어 처리");
            // 층 클리어 처리 → GameManager 호출(이벤트로 묶어?)
        }
    }

    IEnumerator MoveToRoom(Vector2Int roomPos)
    {
        isMoving = true;
        targetPos = new Vector3(roomPos.x, 0.5f, roomPos.y);

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, player.moveSpeed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        yield return new WaitForSeconds(0.2f);

        if (mapGenerator.roomDataMap.TryGetValue(roomPos, out RoomData room) && room.enemies.Count > 0)
        {
            Debug.Log($"전투 시작: {room.enemies.Count}명의 적");
            isFighting = true;
            yield return new WaitForSeconds(1f); // 전투 연출 대체
            room.enemies.Clear();
            isFighting = false;
            Debug.Log("전투 완료");
        }

        //mapGenerator.UpdateVisibleArea(transform.position);
    }

    List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> results = new();
        Vector2Int[] directions = new Vector2Int[] {
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
