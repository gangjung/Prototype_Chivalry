using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterStat{

    protected string name;  // 이름

    protected float str;    // Strength
    protected float def;    // Defence
    protected float dex;    // dex

    protected float hp;
    protected float energy;

    public BaseCharacterStat(string name, float str, float def, float hp, float energy)
    {
        this.name = name;
        this.str = str;
        this.def = def;
        this.dex = dex;
        this.hp = hp;
        this.energy = energy;
    }
    
}
