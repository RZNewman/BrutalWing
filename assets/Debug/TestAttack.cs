using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TestAttack : NetworkBehaviour {
    public float force=300;
    public int damage = 1;
    // Use this for initialization
    void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            other.GetComponent<PlayerMover>().getHit(force, transform.position, damage);
        }
       
       
    }
}
