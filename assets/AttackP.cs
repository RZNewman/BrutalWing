using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackP : Attack {
    public GameObject projPre;
    public bool ground = false;

    bool fired = false;
    protected override void uptake(float cur)
    {
        if (!fired && cur > windUp)
        {
            fired = true;
            GameObject p = Instantiate(projPre);
            if (ground)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, Vector3.down, out hit, 20, 1 << 14);
                p.transform.position = hit.point + transform.forward * 1f + Vector3.up * (p.GetComponent<Collider>().bounds.extents.y+0.1f);
            }
            else
            {
                p.transform.position += transform.position + transform.forward * 1f;
            }
            
            p.transform.rotation = transform.rotation;
            Projectile proj = p.GetComponent<Projectile>();
            proj.setPlayer(player, owner);
            NetworkServer.Spawn(p);
        }
    }

    protected override void visuals()
    {
       //none
    }


}
