using UnityEngine;

public class RoomNode
{
    public Vector2Int pos;  //방의 위치
    public float size; // 방 한 변의 길이 (정사각형 기준)

    public RoomNode(Vector2Int pos, float size = 5f)
    {
        this.pos = pos;
        this.size = size;
    }

    public Vector3 GetRandomPointInRoom()
    {
        float half = size / 2f;
        float x = Random.Range(pos.x - half + 0.5f, pos.x + half - 0.5f);
        float z = Random.Range(pos.y - half + 0.5f, pos.y + half - 0.5f);
        return new Vector3(x, 0, z);
    }

    public bool IsInside(Vector3 worldPos)
    {
        float half = size / 2f;
        return (worldPos.x >= pos.x - half && worldPos.x <= pos.x + half &&
                worldPos.z >= pos.y - half && worldPos.z <= pos.y + half);
    }
}