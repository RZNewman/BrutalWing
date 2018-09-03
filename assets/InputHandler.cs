using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
    public bool up, down, left, right, jump, atk1, atk2, abil1, abil2;
    public Vector3 target, groundTarget;

    // Use this for initialization
    void Start()
    {
        up = false;
        down = false;
        left = false;
        right = false;
        jump = false;
        atk1 = false;
        atk2 = false;
        abil1 = false;
        abil2 = false;
        target = new Vector3();
        groundTarget = new Vector3();
    }
    public bool attacking
    {
        get
        {
            return atk1 || atk2|| abil1 || abil2;
        }
    }
    public struct data
    {
        public bool up, down, left, right, jump, atk1, atk2, abil1, abil2;
        public Vector3 target, groundTarget;
    }

    public void sync(data inp)
    {
        up = inp.up;
        down = inp.down;
        left = inp.left;
        right = inp.right;
        jump = inp.jump;
        atk1 = inp.atk1;
        atk2 = inp.atk2;
        abil1 = inp.abil1;
        abil2 = inp.abil2;
        target = inp.target;
        groundTarget = inp.groundTarget;
       
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
        d.atk2 = atk2;
        d.abil1 = abil1;
        d.abil2 = abil2;
        d.target = target;
        d.groundTarget = groundTarget;
        return d;
    }
    // Update is called once per frame

}
