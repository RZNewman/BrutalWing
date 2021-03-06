﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {
    public float force=10;
    public int damage =1;
    public float speed =9;
    public float vert = 0;
    public float lifetime =2;

    protected int ownerTeam;
    protected GameObject owner;
    float birth;

    public void setPlayer(int t, NetworkInstanceId o)
    {
        ownerTeam = t;
        owner = NetworkServer.FindLocalObject(o);
    }
    public void setPlayer(int t, GameObject o) //Can Only be used inside the server
    {
        ownerTeam = t;
        owner = o;
    }
    // Use this for initialization
    void Start () {
        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 vel = new Vector3();
        vel += transform.forward * speed;
        if (vert != 0)
        {
            rb.useGravity = true;
            vel += transform.up * vert;
        }
        rb.velocity = vel;
        birth = Time.time;
	}

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            //print(owner);
            //hit.Add(other.gameObject);
            PlayerMover pm = other.GetComponent<PlayerMover>();
            if (pm)
            {
                if(pm.team != ownerTeam)
                {
                    other.GetComponent<PlayerMover>().getHit(force, transform.forward, damage, ownerTeam);
                    Destroy(gameObject);
                }
                
            }
            else
            {
                Destroy(gameObject);
            }
            
            



        }
    }
    void Update()
    {
        if (isServer && Time.time > birth + lifetime)
        {
            endLife();
        }
    }
    protected virtual void endLife()
    {
        Destroy(gameObject);
    }
}
