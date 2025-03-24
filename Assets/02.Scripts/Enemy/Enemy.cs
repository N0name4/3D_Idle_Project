using UnityEngine;

[System.Serializable]
public class Enemy
{
    public string type;
    public int maxHP;
    public int atk;
    public int def;

    public Enemy(string type, int maxHP, int atk, int def)
    {
        this.type = type;
        this.maxHP = maxHP;
        this.atk = atk;
        this.def = def;
    }
}