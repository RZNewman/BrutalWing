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
        up = Input.GetKey(KeyCode.W);
        down = Input.GetKey(KeyCode.S);
        left = Input.GetKey(KeyCode.A);
        right = Input.GetKey(KeyCode.D);
        jump = Input.GetKey(KeyCode.Space);
        atk1 = Input.GetMouseButton(0);
        Ray r = cam.ScreenPointToRay(Input.mousePosition);//here
        RaycastHit hits;
        Physics.Raycast(r, out hits, 50, 1 << 8);
        target = hits.point;
    }
}
