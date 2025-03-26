using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAutoExplorer : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public Player player;

    private Stack<Vector2Int> dfsStack = new();
    private HashSet<Vector2Int> visited = new();
    private HashSet<Vector2Int> visitedRoom = new();
    private Stack<Vector2Int> cameFrom = new();
    private bool isMoving = false;
    private bool isFighting = false;

    EnemySpawner enemySpawner;

    IEnumerator Start()
    {
        player = GameManager.Instance.player;
        enemySpawner = GameManager.Instance.enemySpawner;
        yield return new WaitUntil(() => mapGenerator.GetAllRooms().Count > 0);


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
           if (cameFrom.Count > 0)
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
            //플레이어 방향도 회전
            Vector3 dir = (target - transform.position).normalized;
            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
            }

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

        if (IsAllVisited())
        {
            Debug.Log("모든 방 탐험 완료!");
            // GameManager.Instance.OnFloorCleared?.Invoke();
        }

    }
    private IEnumerator CombatCoroutine(RoomData room)
    {
        isFighting = true;

        List<Enemy> roomEnemies = new List<Enemy>(enemySpawner.GetEnemiesInRoom(room.pos));

        if (roomEnemies.Count == 0)
        {
            Debug.Log("전투할 적이 없음");
            isFighting = false;
            yield break;
        }

        foreach (Enemy targetEnemy in roomEnemies)
        {
            if (targetEnemy == null) continue;
            if (player.condition.currentHp <= 0) break;

            Debug.Log($"⚔️ {targetEnemy.characterName} 와 전투 시작!");

            float playerTimer = 0f;
            float enemyTimer = 0f;

            while (player.condition.currentHp > 0 && targetEnemy.condition.currentHp > 0)
            {
                Vector3 directionToEnemy = targetEnemy.transform.position - transform.position;
                directionToEnemy.y = 0f;

                if (directionToEnemy != Vector3.zero)
                {
                    Quaternion rot = Quaternion.LookRotation(directionToEnemy);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, 10f * Time.deltaTime);
                }

                yield return null;

                playerTimer += Time.deltaTime;
                enemyTimer += Time.deltaTime;

                // 플레이어 턴
                if (playerTimer >= player.condition.atkCoolDown)
                {
                    playerTimer = 0f;
                    bool enemyDead = targetEnemy.condition.TakeDamage(player.condition.atk);
                    Debug.Log($"🗡 {player.characterName} ▶ {targetEnemy.characterName} 공격! 남은 HP: {targetEnemy.condition.currentHp}");

                    if (enemyDead)
                    {
                        player.GainReward(targetEnemy.data.rewardExp, targetEnemy.data.rewardGold);

                        Debug.Log($"☠️ {targetEnemy.characterName} 처치됨!");
                        targetEnemy.OnDefeated();
                        enemySpawner.RemoveEnemy(room.pos, targetEnemy);
                        break;
                    }
                }

                // 적 턴
                if (enemyTimer >= targetEnemy.condition.atkCoolDown)
                {
                    enemyTimer = 0f;
                    bool playerDead = player.condition.TakeDamage(targetEnemy.condition.atk);
                    Debug.Log($"{targetEnemy.characterName} ▶ {player.characterName} 공격! 남은 HP: {player.condition.currentHp}");

                    if (playerDead)
                    {
                        Debug.Log($"💀 {player.characterName} 사망! 게임 오버!");
                        isFighting = false;
                        yield break;
                    }
                }
            }

            yield return new WaitForSeconds(0.3f); // 다음 적과의 전투 전 딜레이
        }

        Debug.Log("방의 모든 적 처치 완료!");
        isFighting = false;

        TryClear();
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

    bool IsAllEnemiesDefeated()
    {
        foreach (var list in enemySpawner.activeEnemiesByRoom.Values)
        {
            if (list.Count > 0)
                return false;
        }
        return true;
    }


    void TryClear()
    {
        if (IsAllVisited() && IsAllEnemiesDefeated())
        {
            Debug.Log("던전 클리어! 다음 층으로 이동...");
            StartCoroutine(GoToNextFloor());
        }
    }

    IEnumerator GoToNextFloor()
    {
        yield return new WaitForSeconds(2f);

        GameManager.Instance.currentFloor++;
        GameManager.Instance.CreateNewFloor();

        // DFS 관련 상태 초기화
        dfsStack.Clear();
        visited.Clear();
        visitedRoom.Clear();
        cameFrom.Clear();

        // 플레이어 시작 위치
        Vector2Int start = mapGenerator.GetAllRooms()[0].pos;
        transform.position = new Vector3(start.x, 0.5f, start.y);
        dfsStack.Push(start);

        Debug.Log("다음 층 시작!");
    }
}
