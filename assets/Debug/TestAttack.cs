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
            Vector3 hitDirection = other.gameObject.transform.position - transform.position;
            hitDirection.y = 0;
            hitDirection.Normalize();
            other.GetComponent<PlayerMover>().getHit(force, hitDirection, damage);
        }
       
       
    }
}
