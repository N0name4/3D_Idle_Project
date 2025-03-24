using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition
{
    public int maxHp;
    public int currentHp;
    public int atk;
    public int def;

    public Condition(int hp, int atk, int def)
    {
        this.maxHp = hp;
        this.currentHp = hp;
        this.atk = atk;
        this.def = def;
    }

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
    }

    public bool TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - def, 1);
        currentHp -= finalDamage;
        return currentHp <= 0;
    }
}
