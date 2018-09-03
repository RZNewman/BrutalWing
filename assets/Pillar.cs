using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : Spawnable {
    public float distance;
    public float durration = 0.5f;

    Vector3 inital;
    Vector3 target;
    protected override void Start2()
    {
        inital = transform.position;
        target = inital + Vector3.up * distance;
    }

	
	// Update is called once per frame
	void FixedUpdate () {

        transform.position = Vector3.Lerp(inital, target, (Time.time - birth) / durration);
       

	}
}
