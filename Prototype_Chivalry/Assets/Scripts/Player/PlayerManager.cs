using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private CharacterStat _stat;    // Player's Stat
    private List<int> _buffList;
    // Movement
    // Skill

    private void Awake()
    {
        _stat = new CharacterStat("홍길동", 10, 10, 10, 10, 10, 5);
        // _buffList = new List<>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PlayerMove();
	}

    public void AddBuff(/*버프 정보를 넣자.*/)
    {
        // Todo
        // 버프 번호에 맞는 버프 효과를 찾아서 버프리스트에 추가하고 효과를 적용시켜 준다.
    }

    public void SubBuff(/*버프 정보를 넣자.*/)
    {
        // Todo
        // 버프 번호에 맞는 버프 효과를 찾아서 버프리스트에 제거하고 효과를 없애준다.
    }

    private void PlayerMove()
    {
        float _moveHori = Input.GetAxisRaw("Horizontal");
        float _moveVerti = Input.GetAxisRaw("Vertical");

        if (_moveHori == 0 && _moveVerti == 0)
            return;

        Vector3 movement = new Vector3(_moveHori, 0, _moveVerti) * Time.deltaTime * _stat.Movespeed;
        transform.position += movement;

        Quaternion quaternion = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
           
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            
        }
    }
}