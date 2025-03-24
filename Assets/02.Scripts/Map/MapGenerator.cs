using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorPrefab;      
    public GameObject enemyPrefab;

    [Header("Map Settings")]
    public int roomCount = 15;      //방 개수
    public int renderRadius = 8;        //렌더링할 범위

    [HideInInspector] public HashSet<Vector2Int> floorPositions = new();
    [HideInInspector] public Dictionary<Vector2Int, RoomData> roomDataMap = new();

    private Dictionary<Vector2Int, GameObject> renderedTiles = new();

    //방 정보
    public class RoomData
    {
        public Vector2Int pos;
        public List<Vector3> enemyOffsets = new();
    }

    public void GenerateFloor(Vector2Int startPos)
    {
        foreach (var obj in renderedTiles.Values) Destroy(obj);
        renderedTiles.Clear();
        floorPositions.Clear();
        roomDataMap.Clear();

        List<RoomNode> rooms = new();
        System.Random rand = new();

        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int pos = new(rand.Next(-20, 20), rand.Next(-20, 20));
            rooms.Add(new RoomNode(pos));
            floorPositions.Add(pos);

            RoomData room = new RoomData { position = pos };
            int enemyCount = Random.Range(1, 4);
            for (int j = 0; j < enemyCount; j++)
            {
                room.enemyOffsets.Add(new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
            }

            roomDataMap[pos] = room;
        }

        var edges = DungeonConnector.ConnectRoomsKruskal(rooms);
        foreach (var edge in edges) ConnectRooms(edge.a.position, edge.b.position);
    }

    // 직선 통로로 바닥 연결
    void ConnectRooms(Vector2Int a, Vector2Int b)
    {
        Vector2Int pos = a;
        while (pos.x != b.x)
        {
            pos.x += (b.x > pos.x) ? 1 : -1;
            floorPositions.Add(pos);
        }
        while (pos.y != b.y)
        {
            pos.y += (b.y > pos.y) ? 1 : -1;
            floorPositions.Add(pos);
        }
    }

    // 플레이어 주변만 렌더링
    public void UpdateVisibleArea(Vector2Int playerPos)
    {
        foreach (var pos in floorPositions)
        {
            bool visible = Vector2Int.Distance(playerPos, pos) <= renderRadius;

            if (visible && !renderedTiles.ContainsKey(pos))
            {
                GameObject tile = Instantiate(floorPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                renderedTiles[pos] = tile;

                // 방이면 적 생성
                if (roomDataMap.ContainsKey(pos))
                {
                    foreach (var offset in roomDataMap[pos].enemyOffsets)
                    {
                        GameObject enemy = Instantiate(enemyPrefab, tile.transform.position + offset, Quaternion.identity);
                        enemy.tag = "Enemy";
                    }
                }
            }
            else if (!visible && renderedTiles.ContainsKey(pos))
            {
                Destroy(renderedTiles[pos]);
                renderedTiles.Remove(pos);
            }
        }
    }
}
