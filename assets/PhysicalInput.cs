using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalInput : InputHandler {


    Camera cam;
    // Use this for initialization
    void Start () {
		
	}
    public void setCamera(Camera c)
    {
        cam = c;
    }

    // Update is called once per frame
    public void Poll()
    {
        up = Input.GetButton("Up");
        down = Input.GetButton("Down");
        left = Input.GetButton("Left");
        right = Input.GetButton("Right");
        jump = Input.GetButton("Jump");
        atk1 = Input.GetMouseButton(0);
        atk2 = Input.GetMouseButton(1);
        abil1 = Input.GetButton("Abil1");
        abil2 = Input.GetButton("Abil2");
        /*
        up = Input.GetKey(KeyCode.W);
        down = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        jump = Input.GetKey(KeyCode.Space);
        atk1 = Input.GetMouseButton(0);
        atk2 = Input.GetMouseButton(1);
        */
        if (cam)
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);//here
            RaycastHit hits;
            Physics.Raycast(r, out hits, 50, 1 << 8);
            target = hits.point;
            Physics.Raycast(r, out hits, 50, 1 << 14);
            groundTarget = hits.point;
        }
        
    }
}
