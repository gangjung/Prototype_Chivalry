using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    private Character[] _characters;
    private float _moveSpeed;

    Vector3 temp;

    private void Awake()
    {
        _characters = new Character[3];
        _moveSpeed = 5f;
    }

    // Use this for initialization
    void Start () {
        Object prefab = Resources.Load("Character/Character");

        for (int i = 0; i < 3; i++)
        {
            Transform pos = transform.Find("Position").transform.Find((i+1).ToString());
            Character tmp = ((GameObject)Instantiate(prefab, pos.position, pos.rotation, pos)).GetComponent<Character>();
            tmp.PositionNumber = (i + 1);
            _characters[i] = tmp;
        }
	}
	
	// Update is called once per frame
	void Update () {
        temp = transform.position;

        Move();

        if (Input.GetMouseButton(0) == true)
            _moveSpeed = 10f;
        else
            _moveSpeed = 5f;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            _characters[0].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _characters[1].UseSkill();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            _characters[2].UseSkill();
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

    public void Move()
    {
        float _moveHori = Input.GetAxisRaw("Horizontal");
        float _moveVerti = Input.GetAxisRaw("Vertical");

        if (_moveHori == 0 && _moveVerti == 0)
            return;

        Vector3 movement = new Vector3(_moveHori, 0, _moveVerti) * Time.deltaTime * _moveSpeed;
        transform.position += movement;

        Quaternion quaternion = Quaternion.LookRotation(movement);

        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, 0.5f);
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            //transform.position = temp;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            
        }
    }
}