using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Character/Enemy Data", order = 0)]
public class EnemyData : ScriptableObject
{
    public string characterName;

    [Header("기본 능력치")]
    public int maxHp = 100;
    public int atk = 10;
    public int def = 5;

    [Header("행동 설정")]
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.2f;

    [Header("보상")]
    public int rewardExp = 10;
    public int rewardGold = 5;
}