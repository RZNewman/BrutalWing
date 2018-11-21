using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackB : Attack {

    public GameObject buffPre;
    // Use this for initialization
    bool fired = false;
    protected override void uptake(float cur)
    {
        if (!fired && cur > windUp)
        {
            fired = true;
            GameObject b = Instantiate(buffPre,transform.parent);
            Buff buf = b.GetComponent<Buff>();
            buf.owner = owner;
            NetworkServer.Spawn(b);
        }
    }

    // Update is called once per frame
    protected override void visuals()
    {
        //none
    }
}
