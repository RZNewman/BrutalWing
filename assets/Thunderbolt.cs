using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Thunderbolt : Spawnable
{
    public float windup = 0.35f;
    public float forceVertical = 6f;

    public GameObject column;
    CapsuleCollider col;
    List<GameObject> hit = new List<GameObject>();
    GameObject ownerObj;
    protected override void Start2()
    {
        col = GetComponent<CapsuleCollider>();
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
        transform.Rotate(new Vector3(0, 0, 150f * Time.fixedDeltaTime));
        if (diff > 0)
        {
            column.gameObject.SetActive(true);
            if (isServer)
            {
                
                Collider[] found = Physics.OverlapCapsule(transform.position, transform.position + Vector3.up * col.height, col.radius*transform.localScale.x, 1 << 10);
                foreach (Collider other in found)
                {
                    PlayerMover pm = other.GetComponent<PlayerMover>();
                    if (pm.team != ownerTeam && !hit.Contains(other.gameObject))
                    {
                        hit.Add(other.gameObject);
                        Vector3 hitDirection = other.gameObject.transform.position - transform.position;
                        
                        hitDirection.Normalize();
                        hitDirection.y = 5;
                        hitDirection.Normalize();
                        other.GetComponent<PlayerMover>().getHit(forceVertical, hitDirection, 1, ownerTeam);
                    }
                }
            }

        }


    }
}
