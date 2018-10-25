using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AttackM : Attack {
    public float turnDeg = 70; //M
    public float windDown = 0.2f;//M
    public float force = 10;
    public int damage = 1;


    float atkDur
    {
        get
        {
            return duration - windUp - windDown;
        }
    }
    bool activeH = false;//M
    GameObject vis;//M
    List<GameObject> hit = new List<GameObject>();

    Collider[] cols;

    void Awake()
    {
        cols = GetComponents<Collider>();
    }

    protected override void visuals()
    {
        vis = transform.GetChild(0).gameObject;
        vis.SetActive(false);
    }

    protected override void uptake(float cur)
    {
        if (activeH && cur > windUp + atkDur)
        {
            active(false);
        }
        else if (!activeH && cur > windUp && cur < windUp + atkDur)
        {
            active(true);
        }
    }

    void active(bool stat)
    {
        activeH = stat;

        RpcAtkVis(stat);


        setColliders(stat);

    }
    public void setColliders(bool s)
    {
        foreach (Collider c in cols)
        {
            c.enabled = s;
        }
    }
    [ClientRpc]
    void RpcAtkVis(bool stat)
    {
        vis.SetActive(stat);
    }
    void OnTriggerEnter(Collider other)
    {
        if (isServer && other.gameObject != transform.parent.gameObject && !hit.Contains(other.gameObject))
        {
            //print(owner);
            hit.Add(other.gameObject);
            Vector3 hitDirection = other.gameObject.transform.position - transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();
            other.GetComponent<PlayerMover>().getHit(force, hitDirection, damage, ownerTeam);




        }
    }
}
