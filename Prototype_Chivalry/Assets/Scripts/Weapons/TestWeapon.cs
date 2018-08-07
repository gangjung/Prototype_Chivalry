using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWeapon : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce( transform.localRotation * Vector3.forward * 500, ForceMode.Force);
	}
	
	// Update is called once per frame
	void Update () {
	    	
	}
}
