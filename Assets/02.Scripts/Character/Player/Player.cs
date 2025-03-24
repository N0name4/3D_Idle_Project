using UnityEngine;

[System.Serializable]
public class Player : CharacterBase
{
    public int level = 1;
    public int exp = 0;
    public int gold = 0;
    public float moveSpeed = 5f;

    public override void Init()
    {
        characterName = "Player";
        condition = new Condition(100, 15, 5);
    }

    public void GainReward(int expGain, int goldGain)
    {
        exp += expGain;
        gold += goldGain;
        Debug.Log($"[보상] EXP +{expGain}, GOLD +{goldGain}");
    }

    public override void OnDefeated()
    {
        Debug.Log("플레이어 패배! 게임 오버 처리");
        // 게임 오버 연출
    }
}