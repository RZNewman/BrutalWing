using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
    public bool up, down, left, right, jump, atk1;
    public Vector3 target;

    // Use this for initialization
    void Start()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        jump = false;
        atk1 = false;
        target = new Vector3();
    }
    public struct data
    {
        public bool up, down, left, right, jump, atk1;
        public Vector3 target;
    }

    public void sync(data inp)
    {
        up = inp.up;
        down = inp.down;
        left = inp.left;
        right = inp.right;
        jump = inp.jump;
        atk1 = inp.atk1;
        target = inp.target;
       
    }

    public data export()
    {
        data d = new data();
        d.up = up;
        d.down = down;
        d.left = left;
        d.right = right;
        d.jump = jump;
        d.atk1 = atk1;
        d.target = target;
        return d;
    }
    // Update is called once per frame

}
