using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {
    
    private float _movespeed;
    private Vector3 before;

    private void Awake()
    {
        _movespeed = 5;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PlayerMove();
	}

    private void PlayerMove()
    {
        float _moveHori = Input.GetAxisRaw("Horizontal");
        float _moveVerti = Input.GetAxisRaw("Vertical");

        if (_moveHori == 0 && _moveVerti == 0)
            return;

        before = transform.position;

        Vector3 movement = new Vector3(_moveHori, 0, _moveVerti) * Time.deltaTime * _movespeed;
        transform.position += movement;

        Quaternion quaternion = Quaternion.LookRotation(movement);
        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, 0.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Wall")
        {
            
            transform.position = before;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            
        }
    }
}