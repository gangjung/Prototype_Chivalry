using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 데이터를 Protected로 선언한 이유는, 상위 Stat객체에는 public으로 접근할 수 있게하나, 다른 곳에서는 쉽게 접근 할 수 없도록 하기위해.
 * 
 * 버프를 따로 둔 이유는, 장신구 및 다른 효과로 얻은 스탯을 따로 표기할 수 있는 상황이 나올 수 있기 때문!
 * 
 * Q. Stat을 float 배열로 관리해도 괜찮을까???
 * Stat정보를 float배열로 선언한 뒤, 속성으로 다가갈 수 있도록???
 * 
 */
public class CharacterStat : BasicStat{

    public override string Name { get { return name; } }
    public override float Str { get { return str + str_buff; } }
    public override float Def { get { return def + def_buff; } }
    public override float Agil { get { return agil + agil_buff; } }
    public override float Hp { get { return hp + hp_buff; } }
    public override float Energy { get { return energy + energy_buff; } }
    public override float Movespeed { get { return movespeed + movespeed_buff; } }

    protected string name;  // 이름. 이름은 한 번 건드리면 다시 건드리지 않게 놔두자.
    
    // Buff stat
    protected float str_buff;    // Strength
    protected float def_buff;    // Defence
    protected float agil_buff;    // Agility
    protected float hp_buff;     // Health Point
    protected float energy_buff; // Energy
    protected float movespeed_buff;  // Move speed

    // protected float[] statdatas;
    // protected float[] buffs; // Buffs, Debuffs datas. 만약 두가지를 나누고 싶다면, 따로 배열을 만들면 된다.

    public CharacterStat(string name, float str, float def, float agil, float hp, float energy, float movespeed) : base(name, str, def, agil, hp, energy, movespeed)
    {
        // Buff Stat
        str_buff = 0;
        def_buff = 0;
        agil_buff = 0;
        hp_buff = 0;
        energy_buff = 0;
        movespeed_buff = 0;
    }

    /// <summary>
    /// Get Character's Buff stat data.
    /// </summary>
    /// <returns> float[] buffdata </returns>
    public float[] GetBuffData()
    {
        float[] temp = { str_buff, def_buff, agil_buff, hp_buff, energy_buff, movespeed_buff };

        return temp;
    }

    /// <summary>
    /// Get Character's Total(basic+buff+debuff) stat data.
    /// </summary>
    /// <returns> float[] TotalStatData </returns>
    public float[] GetTotalStatData()
    {
        float[] temp = { str + str_buff, def + def_buff, agil + agil_buff, hp + hp_buff, energy + energy_buff, movespeed + movespeed_buff };

        return temp;
    }

    /// <summary>
    /// Add Character's Buff data
    /// </summary>
    /// <param name="Data"></param>
    /// <returns>
    /// true = Success to add Buff data.
    /// false = Fail to add Buff data
    /// </returns>
    public bool AddBuffData(float[] Data)
    {
        if (Data.Length != _statsize)
        {
            Debug.LogWarning("Data 정보가 올바르지 않습니다.");
            return false;
        }

        // 서버와 통신해서 데이터를 바꿔줘야 한다. 여기서 실패하면, false를 반환
        // 서버와 통신이 잘 됐다면, 클라이언트의 정보도 바꿔준다!
        // 순서는 서버->클라이언트.

        str_buff += Data[0];
        def_buff += Data[1];
        agil_buff += Data[2];
        hp_buff += Data[3];
        energy_buff += Data[4];
        movespeed_buff += Data[5];

        //for(int i = 0; i< _statsize; ++i)
        //{
        //    buffdata[i] += Data[i];
        //}

        return true;
    }

    /// <summary>
    /// Subtract Character's Buff data
    /// </summary>
    /// <param name="Data"></param>
    /// <returns>
    /// true = Success to add Buff data.
    /// false = Fail to add Buff data
    /// </returns>
    public bool SubBuffData(float[] Data)
    {
        if (Data.Length != _statsize)
        {
            Debug.LogWarning("Data 정보가 올바르지 않습니다.");
            return false;
        }

        // 서버와 통신해서 데이터를 바꿔줘야 한다. 여기서 실패하면, false를 반환
        // 서버와 통신이 잘 됐다면, 클라이언트의 정보도 바꿔준다!
        // 순서는 서버->클라이언트.

        str_buff -= Data[0];
        def_buff -= Data[1];
        agil_buff -= Data[2];
        hp_buff -= Data[3];
        energy_buff -= Data[4];
        movespeed_buff -= Data[5];

        //for(int i = 0; i< _statsize; ++i)
        //{
        //    buffdata[i] -= Data[i];
        //}

        return true;
    }
}