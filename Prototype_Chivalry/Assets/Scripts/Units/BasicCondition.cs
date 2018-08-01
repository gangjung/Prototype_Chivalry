using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 상태에 따라서 Coroutine을 걸어줄까???
 * 해당 상태가 걸리면 Coroutine을 걸어주는거고.
 * 만약 중복 상태가 걸리면, 해당 코루틴에 정보를 전달해서 시간을 추가적으로 늘려주는거고 ㅇㅇ
 */
public class BasicCondition{

    public bool IsNoSkill { get { return isNoSkill; } }
    public bool IsConfused { get { return isConfused; } }
    public bool IsBurning { get { return isBurning; } }
    public bool IsHealing { get { return isHealing; } }

    protected bool isNoSkill;
    protected bool isConfused;
    protected bool isBurning;
    protected bool isHealing;

    /// <summary>
    /// Get Basic Condition Datas.
    /// </summary>
    /// <returns> bool[] conditions </returns>
    public bool[] GetConditions()
    {
        bool[] temp = { isNoSkill, isConfused, isBurning, isHealing };
        return temp;
    }
}
