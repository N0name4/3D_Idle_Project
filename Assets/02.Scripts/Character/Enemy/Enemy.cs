using UnityEngine;

[System.Serializable]
public class Enemy : CharacterBase
{
    public int rewardExp;
    public int rewardGold;
    public Vector3 offsetPos;

    public GameObject linkedObject;

    public override void Init()
    {
        // 이미 데이터 주입 받는 경우엔 생략 가능
        // condition = new Condition(50, 10, 2);
    }

    public override void OnDefeated()
    {
        Debug.Log($"{characterName} 처치됨");
        if (linkedObject != null)
        {
            GameObject.Destroy(linkedObject); // 진짜 GameObject 제거
        }
    }
}