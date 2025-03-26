using UnityEngine;

[System.Serializable]
public class Enemy : CharacterBase
{
    public EnemyData data;
    public Vector3 offsetPos;

    private void Awake()
    {
        Init(); // 프리팹 생성 시 자동 초기화
    }

    public override void Init()
    {
        if (data != null)
        {
            characterName = data.characterName;
            condition = new Condition(data.maxHp, data.atk, data.def, data.attackCooldown);
        }
        else
        {
            Debug.LogWarning($"EnemyData가 설정되지 않았습니다. ({gameObject.name})");
        }
    }

    public override void OnDefeated()
    {
        Debug.Log($"{characterName} 처치됨");
        Destroy(gameObject);
    }
}