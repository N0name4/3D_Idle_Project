using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject roomFloorPrefab;
    public GameObject pathFloorPrefab;
    public List<GameObject> enemyPrefabs;
    public List<GameObject> interiorPrefabs;

    [Header("Map Settings")]
    public int roomCount = 10;
    public float defaultRoomSize = 5f;
    public int renderRadius = 8;

    [HideInInspector] public HashSet<Vector2Int> floorPositions = new();
    private List<RoomNode> roomNodes = new();
    public Dictionary<Vector2Int, RoomData> roomDataMap = new();
    private Dictionary<Vector2Int, GameObject> renderedTiles = new();


    private void Start()
    {
        GenerateNewFloor(Vector2Int.zero); // 시작 좌표 기준으로 생성
        RenderAll();
    }
    /// <summary>
    /// 새 맵 생성하기
    /// </summary>
    /// <param name="startPos"></param>
    public void GenerateNewFloor(Vector2Int startPos)
    {
        floorPositions.Clear();
        roomDataMap.Clear();
        roomNodes.Clear();

        System.Random rand = new();

        // 1. 방 생성
        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int pos = Vector2Int.zero;
            bool valid = false;
            int tries = 0;

            while (!valid && tries < 100)
            {
                tries++;
                pos = new Vector2Int(rand.Next(-20, 20), rand.Next(-20, 20));
                valid = true;

                foreach (var existingRoom in roomNodes)
                {
                    if (Vector2Int.Distance(pos, existingRoom.pos) < defaultRoomSize + 1)
                    {
                        valid = false;
                        break;
                    }
                }
            }
            if (!valid) continue;

            var room = new RoomNode(pos, defaultRoomSize);
            roomNodes.Add(room);
            floorPositions.Add(pos);

            var roomData = new RoomData
            {
                pos = pos,
                interialNum = Random.Range(0, interiorPrefabs.Count),
                enemies = new List<EnemyData>()
            };

            int enemyCount = Random.Range(1, 4);
            for (int j = 0; j < enemyCount; j++)
            {
                roomData.enemies.Add(new EnemyData
                {
                    type = "Goblin",
                    offsetPos = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f))
                });
            }

            roomDataMap[pos] = roomData;
        }

        var edges = DungeonConnector.ConnectRoomsKruskal(roomNodes);
        foreach (var edge in edges) ConnectRooms(edge.a.pos, edge.b.pos);
    }

    // 3. 두 방을 ㄱ자 형태로 연결 (바닥 타일 위치만 추가)
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

    /// <summary>
    /// 현재 플레이어 시야에만 있는 방만 렌더링하기
    /// </summary>
    /// <param name="playerPos"></param>
    public void UpdateVisibleArea(Vector3 playerPos)
    {
        Vector2Int playerGrid = Vector2Int.RoundToInt(new Vector2(playerPos.x, playerPos.z));

        foreach (var pos in floorPositions)
        {
            bool visible = Vector2Int.Distance(playerGrid, pos) <= renderRadius;

            if (visible && !renderedTiles.ContainsKey(pos))
            {
                GameObject prefab = roomDataMap.ContainsKey(pos) ? roomFloorPrefab : pathFloorPrefab;
                GameObject tile = Instantiate(prefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                renderedTiles[pos] = tile;

                if (roomDataMap.ContainsKey(pos))
                {
                    RoomData room = roomDataMap[pos];

                    foreach (var enemy in room.enemies)
                    {
                        GameObject prefabEnemy = enemyPrefabs.Find(e => e.name == enemy.type);
                        if (prefabEnemy != null)
                            Instantiate(prefabEnemy, tile.transform.position + enemy.offsetPos, Quaternion.identity);
                    }

                    if (room.interialNum >= 0 && room.interialNum < interiorPrefabs.Count)
                    {
                        Instantiate(interiorPrefabs[room.interialNum], tile.transform.position, Quaternion.identity);
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


    // 4. 플레이어가 들어간 방 노드 판정
    public RoomNode GetCurrentRoomNode(Vector3 playerPos)
    {
        foreach (var room in roomNodes)
        {
            if (IsInsideRoom(playerPos, room))
            {
                return room;
            }
        }
        return null;
    }

    public bool IsInsideRoom(Vector3 playerPos, RoomNode room)
    {
        float half = room.size / 2f;
        Vector3 center = new Vector3(room.pos.x, 0, room.pos.y);

        return (playerPos.x >= center.x - half && playerPos.x <= center.x + half &&
                playerPos.z >= center.z - half && playerPos.z <= center.z + half);
    }
    public List<RoomNode> GetAllRooms() => roomNodes;

    /// <summary>
    /// 디버깅 용 모든 맵 보여주기
    /// </summary>
    public void RenderAll()
    {
        foreach (var pos in floorPositions)
        {
            if (!renderedTiles.ContainsKey(pos))
            {
                GameObject prefab = roomDataMap.ContainsKey(pos) ? roomFloorPrefab : pathFloorPrefab;
                GameObject tile = Instantiate(prefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                renderedTiles[pos] = tile;

                if (roomDataMap.ContainsKey(pos))
                {
                    RoomData room = roomDataMap[pos];

                    foreach (var enemy in room.enemies)
                    {
                        GameObject prefabEnemy = enemyPrefabs.Find(e => e.name == enemy.type);
                        if (prefabEnemy != null)
                            Instantiate(prefabEnemy, tile.transform.position + enemy.offsetPos, Quaternion.identity);
                    }

                    if (room.interialNum >= 0 && room.interialNum < interiorPrefabs.Count)
                    {
                        Instantiate(interiorPrefabs[room.interialNum], tile.transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }
}
