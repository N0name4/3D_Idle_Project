using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class MapNavBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    public void Build()
    {
        if (surface == null)
        {
            Debug.LogError("NavMeshSurface가 연결되지 않았습니다!");
            return;
        }

        Debug.Log("NavMesh 빌드 시작");
        surface.BuildNavMesh();
    }
}
