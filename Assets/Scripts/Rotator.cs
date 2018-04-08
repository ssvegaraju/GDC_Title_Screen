using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// Lol rotate the visualizer to make it look better??
public class Rotator : MonoBehaviour {

    public float rotateSpeed = 20f;
	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + rotateSpeed * Time.deltaTime);
	}
}
