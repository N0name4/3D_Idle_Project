using UnityEngine;

[System.Serializable]
public class Player
{
    public int level = 1;
    public int maxHP = 100;
    public int atk = 10;
    public int def = 5;
    public float moveSpeed = 3f;

    public int currentHP;

    public void Init()
    {
        currentHP = maxHP;
    }
}