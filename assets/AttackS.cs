using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackS : Attack {

    public GameObject spawnPre;
    [HideInInspector]
    public Vector3 target;
    public float range;
    public bool rotate = false;

    bool fired = false;
    protected override void uptake(float cur)
    {
        if (!fired && cur > windUp)
        {
            fired = true;
            GameObject s = Instantiate(spawnPre);
            s.transform.position += target;
            if (rotate)
            {
                s.transform.rotation = transform.rotation;
            }
            
            Spawnable spa = s.GetComponent<Spawnable>();
            spa.setPlayer(ownerTeam, owner);
            NetworkServer.Spawn(s);
        }
    }

    // Update is called once per frame
    protected override void visuals()
    {
        //none
    }

}
