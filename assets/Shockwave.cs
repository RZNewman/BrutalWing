using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Shockwave : Spawnable {
    public float windup = 0.35f;
    public float force = 10f;
    float visExpand = .05f;

    public GameObject expander;
    SphereCollider col;
    List<GameObject> hit = new List<GameObject>();
    GameObject ownerObj;
    protected override void Start2()
    {
         col= GetComponent<SphereCollider>();
        if (isClient)
        {
            ownerObj = ClientScene.FindLocalObject(owner);
        }
        else
        {
            ownerObj = NetworkServer.FindLocalObject(owner);
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float diff = Time.time - (birth + windup);

        if (diff>0)
        {
            expander.transform.localScale = Vector3.Lerp(new Vector3(.2f, .2f, .2f), new Vector3(.9f, .9f, .9f), diff / visExpand);
            if (isServer)
            {
                Collider[] found = Physics.OverlapSphere(transform.position, col.radius*transform.localScale.x, 1 << 10);
                foreach (Collider other in found)
                {
                    PlayerMover pm = other.GetComponent<PlayerMover>();
                    if (pm.team!= ownerTeam && !hit.Contains(other.gameObject))
                    {
                        hit.Add(other.gameObject);
                        Vector3 hitDirection = other.gameObject.transform.position - transform.position;
                        hitDirection.y = 0;
                        hitDirection.Normalize();
                        other.GetComponent<PlayerMover>().getHit(force, hitDirection, 0, ownerTeam);
                    }
                }
            }
            
        }


    }
}