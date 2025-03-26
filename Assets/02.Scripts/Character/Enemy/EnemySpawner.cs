using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public MapGenerator mapGenerator;
    public List<GameObject> enemyPrefabs;

    public Dictionary<Vector2Int, List<Enemy>> activeEnemiesByRoom = new();

    public void SpawnAllEnemies()
    {
        Debug.Log($"스폰 시작, 방 개수: {mapGenerator.roomDataMap.Count}");

        foreach (var kvp in mapGenerator.roomDataMap)
        {
            RoomData room = kvp.Value;
            Vector2Int roomPos = room.pos;
            Vector3 roomWorldPos = new Vector3(roomPos.x, 0, roomPos.y);

            List<Enemy> enemyList = new();

            foreach (var enemyInfo in room.enemies)
            {
                GameObject prefab = enemyPrefabs.Find(e => e.name == enemyInfo.characterName);
                if (prefab == null)
                {
                    Debug.LogWarning($"Enemy prefab '{enemyInfo.characterName}' not found!");
                    continue;
                }

                Vector3 spawnPos = roomWorldPos + enemyInfo.offsetPos;
                GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity, transform);

                Enemy enemy = spawned.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.offsetPos = enemyInfo.offsetPos; // 필요하면
                    enemyList.Add(enemy);
                }

                EnemyAI ai = spawned.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.Init(mapGenerator.roomNodes.Find(r => r.pos == roomPos));
                }
            }

            activeEnemiesByRoom[roomPos] = enemyList;
        }
    }

    public List<Enemy> GetEnemiesInRoom(Vector2Int roomPos)
    {
        return activeEnemiesByRoom.ContainsKey(roomPos) ? activeEnemiesByRoom[roomPos] : new List<Enemy>();
    }

    public void RemoveEnemy(Vector2Int roomPos, Enemy enemy)
    {
        if (activeEnemiesByRoom.ContainsKey(roomPos))
        {
            activeEnemiesByRoom[roomPos].Remove(enemy);
        }
    }
}
