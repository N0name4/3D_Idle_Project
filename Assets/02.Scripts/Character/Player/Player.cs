using UnityEngine;

[System.Serializable]
public class Player : CharacterBase
{
    public int level = 1;
    public int maxMP = 100;
    public int mp = 100;
    public int maxExp = 100;
    public int exp = 0;
    public int gold = 0;
    public float moveSpeed = 5f;

    public ItemData_Weapon equippedWeapon;
    public ItemData_Armor equippedArmor;

    private void Start()
    {
        Init();
    }

    public override void Init()
    {
        characterName = "Player";
        condition = new Condition(100, 15, 5, 1);
    }

    public override void OnDefeated()
    {
        Debug.Log("플레이어 패배! 게임 오버 처리");
        // 게임 오버 연출
    }

    public void GainReward(int expGain, int goldGain)
    {
        exp += expGain;
        gold += goldGain;

        while (exp >= maxExp)
        {
            exp -= maxExp;
            level++;
            maxExp += 50; // 점점 레벨업 어렵게
            Debug.Log($"레벨업! 현재 레벨: {level}");
        }

        Debug.Log($"[보상] EXP +{expGain}, GOLD +{goldGain}");
    }

    public void EquipWeapon(ItemData_Weapon weapon)
    {
        if (equippedWeapon != null)
            condition.atk -= equippedWeapon.bonusAtk;

        equippedWeapon = weapon;
        condition.atk += weapon.bonusAtk;

        Debug.Log($"무기 장착: {weapon.itemName} (+{weapon.bonusAtk} ATK)");
    }

    public void EquipArmor(ItemData_Armor armor)
    {
        if (equippedArmor != null)
        {
            condition.def -= equippedArmor.bonusDef;
            condition.maxHp -= equippedArmor.bonusHp;
        }

        equippedArmor = armor;
        condition.def += armor.bonusDef;
        condition.maxHp += armor.bonusHp;
        condition.currentHp = Mathf.Min(condition.currentHp, condition.maxHp);

        Debug.Log($"방어구 장착: {armor.itemName} (+{armor.bonusDef} DEF, +{armor.bonusHp} HP)");
    }
}