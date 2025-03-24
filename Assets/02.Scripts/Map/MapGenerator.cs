using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject floorPrefab;

    [Header("Map Settings")]
    public int roomCount = 10;
    public float defaultRoomSize = 5f;

    [HideInInspector] public HashSet<Vector2Int> floorPositions = new();
    private List<RoomNode> roomNodes = new();


    /// <summary>
    /// 새 맵 생성하기
    /// </summary>
    /// <param name="startPos"></param>
    public void GenerateNewFloor(Vector2Int startPos)
    {
        floorPositions.Clear();
        roomNodes.Clear();

        System.Random rand = new();

        // 1. 방 생성
        for (int i = 0; i < roomCount; i++)
        {
            Vector2Int pos = new(rand.Next(-20, 20), rand.Next(-20, 20));
            if (floorPositions.Contains(pos)) continue;

            RoomNode room = new RoomNode(pos, defaultRoomSize);
            roomNodes.Add(room);
            floorPositions.Add(pos);
        }

        // 2. MST로 방 연결
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
}