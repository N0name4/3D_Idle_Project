using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase
{
    public string characterName;
    public Condition condition;

    public abstract void Init();
    public abstract void OnDefeated();
}
