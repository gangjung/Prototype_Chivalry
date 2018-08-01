using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class CameraMove : MonoBehaviour {

    private GameObject _player;
    [Range(-100,100)]
    public float x;
    [Range(-100, 100)]
    public float y;
    [Range(-100, 100)]
    public float z;

    private void Awake()
    {
        _player = GameObject.Find("Player");

        x = _player.transform.position.x;
        y = _player.transform.position.y + 4;
        z = _player.transform.position.z - 5;

        transform.position = new Vector3(x, y, z);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        //transform.position = _player.transform.position + new Vector3(0, 4, -5);
        transform.position = Vector3.Lerp(transform.position, _player.transform.position + new Vector3(0, 4, -5), 0.1f);
    }
}
