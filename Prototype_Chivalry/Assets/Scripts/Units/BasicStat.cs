using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * You can use this StatData in EveryWhere. ex) Character, Item, Skill etc.
 */
public class BasicStat{

    public virtual string Name { get { return name; } }
    public virtual float Str { get { return str; } }
    public virtual float Def { get { return def; } }
    public virtual float Agil { get { return agil; } }
    public virtual float Hp { get { return hp; } }
    public virtual float Energy { get { return energy; } }
    public virtual float Movespeed { get { return movespeed; } }

    protected string name;  // 이름. 이름은 한 번 건드리면 다시 건드리지 않게 놔두자.

    protected int _statsize = 6;
    // Stat Size : 6
    protected float str;    // Strength
    protected float def;    // Defence
    protected float agil;    // Agility
    protected float hp;     // Health Point
    protected float energy; // Energy
    protected float movespeed;  // Move speed

    public BasicStat(string name, float str, float def, float agil, float hp, float energy, float movespeed)
    {
        // Basic Stat
        this.name = name;
        this.str = str;
        this.def = def;
        this.agil = agil;
        this.hp = hp;
        this.energy = energy;
        this.movespeed = movespeed;
    }

    /// <summary>
    /// Get Basic data.
    /// </summary>
    /// /// <returns> float[] statdata </returns>
    public float[] GetBasicStatData()
    {
        float[] temp = { str, def, agil, hp, energy, movespeed };

        return temp;
    }

    /// <summary>
    /// Set Basic Stat data
    /// </summary>
    /// <param name="Data"></param>
    /// <returns>
    /// true = Success to update Stat data.
    /// false = Fail to update Stat data
    /// </returns>
    public bool SetBasicStatData(float[] Data)
    {
        if (Data.Length != _statsize)
        {
            Debug.LogWarning("Data 정보가 올바르지 않습니다.");
            return false;
        }

        // 서버와 통신해서 데이터를 바꿔줘야 한다. 여기서 실패하면, false를 반환
        // 서버와 통신이 잘 됐다면, 클라이언트의 정보도 바꿔준다!
        // 순서는 서버->클라이언트.

        str = Data[0];
        def = Data[1];
        agil = Data[2];
        hp = Data[3];
        energy = Data[4];
        movespeed = Data[5];

        //for(int i = 0; i< _statsize; ++i)
        //{
        //    statdata[i] = Data[i];
        //}

        return true;
    }
}
