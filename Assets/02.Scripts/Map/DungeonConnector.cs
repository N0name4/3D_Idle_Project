using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonConnector : MonoBehaviour
{
    public static List<Edge> ConnectRoomsKruskal(List<RoomNode> rooms)
    {
        var edges = new List<Edge>();
        var parent = new Dictionary<RoomNode, RoomNode>();

        foreach (var r in rooms) parent[r] = r;

        RoomNode Find(RoomNode x)
        {
            if (parent[x] != x) parent[x] = Find(parent[x]);
            return parent[x];
        }

        void Union(RoomNode a, RoomNode b)
        {
            var rootA = Find(a);
            var rootB = Find(b);
            if (rootA != rootB)
                parent[rootB] = rootA;
        }

        for (int i = 0; i < rooms.Count; i++)
        {
            for (int j = i + 1; j < rooms.Count; j++)
            {
                edges.Add(new Edge(rooms[i], rooms[j]));
            }
        }

        edges.Sort((e1, e2) => e1.cost.CompareTo(e2.cost));

        var mst = new List<Edge>();
        foreach (var edge in edges)
        {
            if (Find(edge.a) != Find(edge.b))
            {
                mst.Add(edge);
                Union(edge.a, edge.b);
            }
        }
        return mst;
    }
}