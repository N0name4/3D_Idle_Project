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
}