using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    public Enemy enemy;
    public void Init(Enemy enemyInstance)
    {
        enemy = enemyInstance;
        enemy.data = enemyData; // 반드시 데이터 연결
        enemy.Init();           // 상태 초기화
    }
}
