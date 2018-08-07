using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 애니메이션을 여기랑 연관되게!
/// </summary>
public class CharacterMovement {

    // Property
    public float ReturnToPlayerSpeed { get { return _returnToPlayerSpeed; } }
    public bool Skill { get { return _skill; } set { _skill = value; } }
    public bool Return { get { return _return; } set { _return = value; } }
    // public float MoveSpeed { set {_moveSpeed = value; } }

    private Transform _character;
    private Transform _position;
    private float _returnToPlayerSpeed;
    private bool _move;
    private bool _stand;
    private bool _slow;
    private bool _skill;    // 스킬 사용중을 따로 하거나, 아니면 stand라고 하고 return만 상태 바꾸던가.
    private bool _return;

    public CharacterMovement(Transform character, Transform position)
    {
        _character = character;
        _position = position;
        Init();
    }
    
    private void Init()
    {
        _returnToPlayerSpeed = 7;
        _move = false;
        _stand = false;
        _slow = false;
        _skill = false;
    }

    public void Move()
    {
        
    }

    public void Attack()
    {

    }

    public void UseSkill()
    {
        _skill = true;
    }

    public void Attacked()
    {

    }

    public void Returning()
    {
        if (_return == true)
        {
            Quaternion quaternion = Quaternion.LookRotation(_position.position - _character.position);
            _character.rotation = quaternion;
            _character.position = Vector3.MoveTowards(_character.position, _position.position, _returnToPlayerSpeed * Time.deltaTime);

            if (Vector3.Distance(_character.position, _position.position) < 0.5f)
            {
                _character.SetParent(_position);
                _character.SetPositionAndRotation(_position.position, _position.rotation);

                _return = false;
                _skill = false;
            }
        }
    }
}
