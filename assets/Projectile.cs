using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour {
    public float force=10;
    public int damage =1;
    public float speed =9;
    public float vert = 0;
    public float lifetime =2;

    int ownerTeam;
    GameObject owner;
    float birth;

    public void setPlayer(int t, NetworkInstanceId o)
    {
        ownerTeam = t;
        owner = NetworkServer.FindLocalObject(o);
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
        if (isServer && other.gameObject != owner)
        {
            //print(owner);
            //hit.Add(other.gameObject);
            if (other.GetComponent<PlayerMover>())
            {
                other.GetComponent<PlayerMover>().getHit(force, transform.forward, damage, ownerTeam);
            }
            
            Destroy(gameObject);



        }
    }
    void Update()
    {
        if (isServer && Time.time > birth + lifetime)
        {
            endLife();
        }
    }
    protected void endLife()
    {
        Destroy(gameObject);
    }
}
