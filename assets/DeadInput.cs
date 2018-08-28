using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadInput : InputHandler {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        jump = false;
        atk1 = false;
        
        target = Vector3.zero;
    }
}
