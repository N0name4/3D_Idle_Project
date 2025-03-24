using UnityEngine;


/// <summary>
/// 방 사이의 길에 대한 정보를 담음
/// </summary>
public class Edge
{
    public RoomNode a, b;
    public float cost;

    public Edge(RoomNode a, RoomNode b)
    {
        this.a = a;
        this.b = b;
        cost = Vector2Int.Distance(a.pos, b.pos);
    }
}